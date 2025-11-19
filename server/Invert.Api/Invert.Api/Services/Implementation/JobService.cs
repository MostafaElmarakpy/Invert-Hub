using AutoMapper;
using Invert.Api.Data;
using Invert.Api.Dtos;
using Invert.Api.Dtos.Article;
using Invert.Api.Dtos.Job;
using Invert.Api.Entities;
using Invert.Api.Repositories.Implementation;
using Invert.Api.Repositories.Interface;
using Invert.Api.Services.Interface;
using System.Text.Json;

namespace Invert.Api.Services.Implementation
{
    public class JobService : IJobService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticleService> _logger;

        public JobService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ArticleService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }



        public async Task<int> CreateJobAsync(CreateJobDto dto)
        {
            try
            {
                var job = _mapper.Map<Job>(dto);
                job.DatePosted = dto.DatePosted ?? DateTime.UtcNow;

                await _unitOfWork.Job.Add(job);
                await _unitOfWork.Save();

                _logger.LogInformation("Job created successfully with ID: {JobId}", job.Id);
                return job.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job");
                throw;
            }
        }



        public async Task<IEnumerable<JobDto>> GetAllAsync()
        {
            try
            {
                var jobs = await _unitOfWork.Job.GetAll();
                return _mapper.Map<List<JobDto>>(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all jobs");
                throw;
            }


        }

        public async Task<JobDto?> GetByIdAsync(int id)
        {
            try
            {
                var job = await _unitOfWork.Job.Get(j => j.Id == id);
                return job == null ? null : _mapper.Map<JobDto>(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job with ID: {JobId}", id);
                throw;
            }

        }

        public async Task UpdateJobAsync(int id, UpdateJobDto dto)
        {
            try
            {
                var job = await _unitOfWork.Job.Get(j => j.Id == id);
                if (job == null)
                    throw new KeyNotFoundException($"Job with id {id} not found");

                // Use AutoMapper to update only non-null properties
                var updatedJob = _mapper.Map(dto, job);
                // Update timestamp
                // updatedJob.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Job.Update(updatedJob);
                await _unitOfWork.Save();

                _logger.LogInformation("Job updated successfully with ID: {JobId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job with ID: {JobId}", id);
                throw;
            }

        }
        public async Task DeleteJobAsync(int id)
        {
            try
            {
                var job = await _unitOfWork.Job.Get(j => j.Id == id);
                if (job == null)
                    throw new KeyNotFoundException($"Job with id {id} not found");

                _unitOfWork.Job.Remove(job);
                await _unitOfWork.Save();

                _logger.LogInformation("Job deleted successfully with ID: {JobId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job with ID: {JobId}", id);
                throw;
            }
        }
    }
}