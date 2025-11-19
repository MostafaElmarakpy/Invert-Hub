using AutoMapper;
using Invert.Api.Controllers;
using Invert.Api.Data;
using Invert.Api.Dtos;
using Invert.Api.Dtos.Article;
using Invert.Api.Dtos.Project;
using Invert.Api.Entities;
using Invert.Api.Repositories.Implementation;
using Invert.Api.Repositories.Interface;
using Invert.Api.Services.Interface;
using Microsoft.Extensions.Logging;

namespace Invert.Api.Services.Implementation
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProjectService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;

        }

        public async Task<int> CreateAsync(CreateProjectDto dto)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(dto.Title))
                {
                    throw new ArgumentException("Title is required.");
                }

                if (string.IsNullOrWhiteSpace(dto.Description))
                {
                    throw new ArgumentException("Description is required.");
                }

                // ✅ Using AutoMapper for mapping
                var project = _mapper.Map<Project>(dto);
                project.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Project.Add(project);
                await _unitOfWork.Save();

                _logger.LogInformation("Project created successfully with ID: {ProjectId}", project.Id);

                return project.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                throw;
            }
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync()
        {
            try
            {
                var projects = await _unitOfWork.Project.GetAll();

                // ✅ Fixed: Using AutoMapper instead of manual mapping
                var projectDtos = _mapper.Map<IEnumerable<ProjectDto>>(projects);

                _logger.LogInformation("Retrieved {Count} projects", projectDtos.Count());

                return projectDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all projects");
                throw;
            }

        }

        public async Task UpdateAsync(int id, UpdateProjectDto dto)
        {
            try
            {
                var project = await _unitOfWork.Project.Get(p => p.Id == id);

                if (project == null)
                {
                    throw new KeyNotFoundException($"Project with ID {id} not found");
                }

                // ✅ Using AutoMapper for partial updates
                _mapper.Map(dto, project);

                // Update timestamp
                project.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Project.Update(project);
                await _unitOfWork.Save();

                _logger.LogInformation("Project {ProjectId} updated successfully", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project with ID {ProjectId}", id);
                throw;
            }

        }
        public async Task DeleteAsync(int id)
        {
            try
            {
                var project = await _unitOfWork.Project.Get(p => p.Id == id);

                if (project == null)
                {
                    throw new KeyNotFoundException($"Project with ID {id} not found");
                }

                _unitOfWork.Project.Remove(project);
                await _unitOfWork.Save();

                _logger.LogInformation("Project {ProjectId} deleted successfully", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project with ID {ProjectId}", id);
                throw;
            }
        }

        public async Task<ProjectDto> GetByIdAsync(int id)
        {
            try
            {
                // ✅ Fixed: Added async keyword and await
                var project = await _unitOfWork.Project.Get(p => p.Id == id);

                if (project == null)
                {
                    _logger.LogWarning("Project with ID {ProjectId} not found", id);
                    return null;
                }

                // ✅ Fixed: Using AutoMapper for mapping
                var projectDto = _mapper.Map<ProjectDto>(project);

                // ✅ Fixed: Direct return instead of Task.FromResult
                return projectDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project with ID {ProjectId}", id);
                throw;
            }
        }


    
    }
}