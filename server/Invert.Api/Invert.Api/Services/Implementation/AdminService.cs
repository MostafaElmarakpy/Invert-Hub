using AutoMapper;
using Invert.Api.Dtos;
using Invert.Api.Dtos.Admin;
using Invert.Api.Dtos.Article;
using Invert.Api.Dtos.Job;
using Invert.Api.Dtos.Project;
using Invert.Api.Entities;
using Invert.Api.Repositories.Implementation;
using Invert.Api.Repositories.Interface;
using Invert.Api.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Invert.Api.Services.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AdminService> logger
            
            
            )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        #region User Management

        public async Task<IEnumerable<UserManagementDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();

                var userDtos = new List<UserManagementDto>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    userDtos.Add(new UserManagementDto
                    {
                        Id = user.Id,
                        UserName = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        EmailConfirmed = user.EmailConfirmed,
                        IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow,
                        Created = user.Created,
                        LastLogin = user.LastLoginAt,
                        Roles = roles
                    });
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw;
            }
        }

        public async Task<UserManagementDto?> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return null;
                }

                var roles = await _userManager.GetRolesAsync(user);

                return new UserManagementDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    EmailConfirmed = user.EmailConfirmed,
                    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow,
                    Created = user.Created,
                    LastLogin = user.LastLoginAt,
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return false;
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to delete user {UserId}: {Errors}",
                        userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return false;
                }

                _logger.LogInformation("User {UserId} deleted successfully", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> LockUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return false;
                }

                // Lock user for 100 years (effectively permanent)
                var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to lock user {UserId}", userId);
                    return false;
                }

                _logger.LogInformation("User {UserId} locked successfully", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UnlockUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return false;
                }

                var result = await _userManager.SetLockoutEndDateAsync(user, null);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to unlock user {UserId}", userId);
                    return false;
                }

                _logger.LogInformation("User {UserId} unlocked successfully", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> AssignRoleAsync(string userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return false;
                }

                var result = await _userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to assign role {Role} to user {UserId}: {Errors}",
                        role, userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return false;
                }

                _logger.LogInformation("Role {Role} assigned to user {UserId}", role, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {Role} to user {UserId}", role, userId);
                return false;
            }
        }

        public async Task<bool> RemoveRoleAsync(string userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return false;
                }

                var result = await _userManager.RemoveFromRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to remove role {Role} from user {UserId}: {Errors}",
                        role, userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return false;
                }

                _logger.LogInformation("Role {Role} removed from user {UserId}", role, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {Role} from user {UserId}", role, userId);
                return false;
            }
        }

        #endregion

        #region Statistics

        public async Task<AdminStatisticsDto> GetStatisticsAsync()
        {
            try
            {
                var totalUsers = await _userManager.Users.CountAsync();
                var totalArticles = (await _unitOfWork.Article.GetAll()).Count();
                var totalJobs = (await _unitOfWork.Job.GetAll()).Count();
                var totalProjects = (await _unitOfWork.Project.GetAll()).Count();

                // Calculate active users (logged in within last 30 days)
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
                var activeUsers = await _userManager.Users
                    .Where(u => u.LastLoginAt.HasValue && u.LastLoginAt.Value >= thirtyDaysAgo)
                    .CountAsync();

                var lockedUsers = await _userManager.Users
                    .Where(u => u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.UtcNow)
                    .CountAsync();

                return new AdminStatisticsDto
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    LockedUsers = lockedUsers,
                    TotalArticles = totalArticles,
                    TotalJobs = totalJobs,
                    TotalProjects = totalProjects
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin statistics");
                throw;
            }
        }

        #endregion

        #region Article Management

        public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync()
        {
            try
            {
                var articles = await _unitOfWork.Article.GetAll();

                return articles.Select(ArticleDto.FromEntity).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all articles");
                throw;
            }
        }

        public async Task<bool> ApproveArticleAsync(Guid articleId)
        {
            try
            {
                var article = await _unitOfWork.Article.Get(a => a.Id == articleId);
                if (article == null)
                {
                    _logger.LogWarning("Article {ArticleId} not found", articleId);
                    return false;
                }

                // Note: You may need to add Status property to Article entity
                article.Status = "Approved";
                article.ApprovedAt = DateTime.UtcNow;

                _unitOfWork.Article.Update(article);
                await _unitOfWork.Save();

                _logger.LogInformation("Article {ArticleId} approved", articleId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving article {ArticleId}", articleId);
                return false;
            }
        }

        public async Task<bool> RejectArticleAsync(Guid articleId)
        {
            try
            {
                var article = await _unitOfWork.Article.Get(a => a.Id == articleId);
                if (article == null)
                {
                    _logger.LogWarning("Article {ArticleId} not found", articleId);
                    return false;
                }

                // Note: You may need to add Status property to Article entity
                 article.Status = "Rejected";

                _unitOfWork.Article.Update(article);
                await _unitOfWork.Save();

                _logger.LogInformation("Article {ArticleId} rejected", articleId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting article {ArticleId}", articleId);
                return false;
            }
        }

        #endregion

        #region Job Management

        public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
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

        #endregion

        #region Project Management

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
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

        public async Task<bool> ApproveProjectAsync(int projectId)
        {
            try
            {
                var project = await _unitOfWork.Project.Get(p => p.Id == projectId);
                if (project == null)
                {
                    _logger.LogWarning("Project {ProjectId} not found", projectId);
                    return false;
                }

                // Note: Add Status property to Project entity if needed
                 project.Status = "Approved";
                project.ApprovedAt = DateTime.UtcNow;

                _unitOfWork.Project.Update(project);
                await _unitOfWork.Save();

                _logger.LogInformation("Project {ProjectId} approved", projectId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving project {ProjectId}", projectId);
                return false;
            }
        }

        public async Task<bool> RejectProjectAsync(int projectId)
        {
            try
            {
                var project = await _unitOfWork.Project.Get(p => p.Id == projectId);
                if (project == null)
                {
                    _logger.LogWarning("Project {ProjectId} not found", projectId);
                    return false;
                }

                // Note: Add Status property to Project entity if needed
                 project.Status = "Rejected";

                _unitOfWork.Project.Update(project);
                await _unitOfWork.Save();

                _logger.LogInformation("Project {ProjectId} rejected", projectId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting project {ProjectId}", projectId);
                return false;
            }
        }

        #endregion

    }
}
