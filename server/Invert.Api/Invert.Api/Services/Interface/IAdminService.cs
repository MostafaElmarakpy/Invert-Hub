using Invert.Api.Dtos;
using Invert.Api.Dtos.Admin;
using Invert.Api.Dtos.Article;
using Invert.Api.Dtos.Job;
using Invert.Api.Dtos.Project;
using Invert.Api.Entities;

namespace Invert.Api.Services.Interface
{
    public interface IAdminService
    {
        // User Management
        Task<IEnumerable<UserManagementDto>> GetAllUsersAsync();
        Task<UserManagementDto?> GetUserByIdAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> LockUserAsync(string userId);
        Task<bool> UnlockUserAsync(string userId);
        Task<bool> AssignRoleAsync(string userId, string role);
        Task<bool> RemoveRoleAsync(string userId, string role);

        // Statistics
        Task<AdminStatisticsDto> GetStatisticsAsync();

        // Content Management
        Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();
        Task<bool> ApproveArticleAsync(Guid articleId);
        Task<bool> RejectArticleAsync(Guid articleId);

        Task<IEnumerable<JobDto>> GetAllJobsAsync();
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
        Task<bool> ApproveProjectAsync(int projectId);
        Task<bool> RejectProjectAsync(int projectId);
    }
}
