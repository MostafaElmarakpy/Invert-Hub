using Invert.Api.Dtos.Article;
using Invert.Api.Dtos.Project;
using Invert.Api.Entities;
using Invert.Api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Invert.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _service;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ProjectController> _logger;
    public ProjectController(
          IProjectService service,
          IMemoryCache cache,
          ILogger<ProjectController> logger)
    {
        _service = service;
        _cache = cache;
        _logger = logger;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            const string cacheKey = "all_projects";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<ProjectDto> projects))
            {
                projects = await _service.GetAllAsync();
                _cache.Set(cacheKey, projects, TimeSpan.FromMinutes(5));
                _logger.LogInformation("Projects loaded from database and cached");
            }
            else
            {
                _logger.LogInformation("Projects loaded from cache");
            }

            return Ok(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all projects");
            return StatusCode(500, "Internal server error while retrieving projects");
        }
    }


    [HttpPost]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ✅ Fixed: Added await keyword
            var projectId = await _service.CreateAsync(dto);

            // Clear cache after creating new project
            _cache.Remove("all_projects");
            _logger.LogInformation("Project created successfully with ID: {ProjectId}", projectId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = projectId },
                new { ProjectId = projectId, Message = "Project created successfully." }
            );
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error while creating project");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            return StatusCode(500, "Internal server error while creating project");
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid project ID");
            }

            var cacheKey = $"project_{id}";

            if (!_cache.TryGetValue(cacheKey, out ProjectDto project))
            {
                project = await _service.GetByIdAsync(id);
                if (project != null)
                {
                    _cache.Set(cacheKey, project, TimeSpan.FromMinutes(10));
                    _logger.LogInformation("Project {ProjectId} loaded from database and cached", id);
                }
            }
            else
            {
                _logger.LogInformation("Project {ProjectId} loaded from cache", id);
            }

            if (project == null)
            {
                _logger.LogWarning("Project with ID {ProjectId} not found", id);
                return NotFound($"Project with ID {id} not found");
            }

            return Ok(project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project with ID {ProjectId}", id);
            return StatusCode(500, "Internal server error while retrieving project");
        }
    }

    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult>  Update(int id, [FromBody] UpdateProjectDto dto)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid project ID");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            var existingProject = await _service.GetByIdAsync(id);

            if (existingProject == null)
            {
                _logger.LogWarning("Project with ID {ProjectId} not found for update", id);
                return NotFound($"Project with ID {id} not found");
            }

            await _service.UpdateAsync(id, dto);

            // Clear relevant cache entries
            _cache.Remove($"project_{id}");
            _cache.Remove("all_projects");

            _logger.LogInformation("Project {ProjectId} updated successfully", id);

            return Ok(new { Message = "Project updated successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project with ID {ProjectId} not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project with ID {ProjectId}", id);
            return StatusCode(500, "Internal server error while updating project");
        }
    }

    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid project ID");
            }

            await _service.DeleteAsync(id);

            // Clear cache entries
            _cache.Remove($"project_{id}");
            _cache.Remove("all_projects");

            _logger.LogInformation("Project {ProjectId} deleted successfully", id);

            return Ok(new { Message = "Project deleted successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project with ID {ProjectId} not found for deletion", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project with ID {ProjectId}", id);
            return StatusCode(500, "Internal server error while deleting project");
        }
    }
}