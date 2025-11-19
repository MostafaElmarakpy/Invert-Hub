using Invert.Api.Dtos.Article;
using Invert.Api.Dtos.Job;
using Invert.Api.Dtos.Project;
using Invert.Api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Invert.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _service;
    private readonly IMemoryCache _cache;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IJobService service, IMemoryCache cache, ILogger<JobsController> logger)
    {
        _service = service;
        _cache = cache;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        //get all projects by ProjectService
        var jobs = await _service.GetAllAsync();
        return Ok(jobs);
    }


    [HttpPost]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateJobDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var jobId = await _service.CreateJobAsync(dto);

             //Clear cache after creating new job
            _cache.Remove("all_jobs");

            return CreatedAtAction(nameof(GetById), new { id = jobId },
                new { JobId = jobId, Message = "Job created successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job");
            return BadRequest(ex.Message);
        }

    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var cacheKey = $"job_{id}";

            if (!_cache.TryGetValue(cacheKey, out JobDto job))
            {
                job = await _service.GetByIdAsync(id);
                if (job != null)
                {
                    _cache.Set(cacheKey, job, TimeSpan.FromMinutes(10));
                }
            }

            if (job == null)
                return NotFound($"Job with ID {id} not found");

            return Ok(job);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job with ID {JobId}", id);
            return StatusCode(500, "Internal server error");
        }


    }

    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult>  Update(int id, [FromBody] UpdateJobDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // FIX: Add await keyword here - this was the main issue causing the 400 error
            var existingJob = await _service.GetByIdAsync(id);
            if (existingJob == null)
                return NotFound($"Job with ID {id} not found");

            await _service.UpdateJobAsync(id, dto);

            // Clear relevant cache entries
            _cache.Remove($"job_{id}");
            _cache.Remove("all_jobs");

            return Ok(new { Message = "Job updated successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job with ID {JobId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Invalid job ID");

            await _service.DeleteJobAsync(id);

            // Clear cache entries
            _cache.Remove($"job_{id}");
            _cache.Remove("all_jobs");

            return Ok(new { Message = "Job deleted successfully." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Job with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job with ID {JobId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}