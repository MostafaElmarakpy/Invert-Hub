using Invert.Api.Dtos.Admin;
using Invert.Api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invert.Api.Controllers
{
    /// <summary>
    /// Admin management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")] // Only admins can access
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IAdminService adminService,
            ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        #region User Management

        /// <summary>
        /// Get all users in the system
        /// </summary>
        [HttpGet("users")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                return Ok(new { users });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(500, new { message = "Failed to retrieve users" });
            }
        }

        /// <summary>
        /// Get specific user details
        /// </summary>
        [HttpGet("users/{userId}")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(string userId)
        {
            try
            {
                var user = await _adminService.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", userId);
                return StatusCode(500, new { message = "Failed to retrieve user" });
            }
        }

        /// <summary>
        /// Delete user from system
        /// </summary>
        [HttpDelete("users/{userId}")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var result = await _adminService.DeleteUserAsync(userId);

                if (!result)
                    return NotFound(new { message = "User not found" });

                _logger.LogInformation("User {UserId} deleted by admin", userId);
                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return StatusCode(500, new { message = "Failed to delete user" });
            }
        }

        /// <summary>
        /// Lock user account
        /// </summary>
        [HttpPost("users/{userId}/lock")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LockUser(string userId)
        {
            try
            {
                var result = await _adminService.LockUserAsync(userId);

                if (!result)
                    return NotFound(new { message = "User not found" });

                _logger.LogInformation("User {UserId} locked by admin", userId);
                return Ok(new { message = "User locked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking user {UserId}", userId);
                return StatusCode(500, new { message = "Failed to lock user" });
            }
        }

        /// <summary>
        /// Unlock user account
        /// </summary>
        [HttpPost("users/{userId}/unlock")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            try
            {
                var result = await _adminService.UnlockUserAsync(userId);

                if (!result)
                    return NotFound(new { message = "User not found" });

                _logger.LogInformation("User {UserId} unlocked by admin", userId);
                return Ok(new { message = "User unlocked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking user {UserId}", userId);
                return StatusCode(500, new { message = "Failed to unlock user" });
            }
        }

        /// <summary>
        /// Assign role to user
        /// </summary>
        [HttpPost("users/{userId}/roles")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignRole(string userId, [FromBody] AssignRoleDto dto)
        {
            try
            {
                var result = await _adminService.AssignRoleAsync(userId, dto.Role);

                if (!result)
                    return BadRequest(new { message = "Failed to assign role" });

                _logger.LogInformation("Role {Role} assigned to user {UserId}", dto.Role, userId);
                return Ok(new { message = $"Role '{dto.Role}' assigned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role to user {UserId}", userId);
                return StatusCode(500, new { message = "Failed to assign role" });
            }
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        [HttpDelete("users/{userId}/roles/{role}")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            try
            {
                var result = await _adminService.RemoveRoleAsync(userId, role);

                if (!result)
                    return BadRequest(new { message = "Failed to remove role" });

                _logger.LogInformation("Role {Role} removed from user {UserId}", role, userId);
                return Ok(new { message = $"Role '{role}' removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role from user {UserId}", userId);
                return StatusCode(500, new { message = "Failed to remove role" });
            }
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Get admin dashboard statistics
        /// </summary>
        [HttpGet("statistics")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var stats = await _adminService.GetStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics");
                return StatusCode(500, new { message = "Failed to retrieve statistics" });
            }
        }

        #endregion

        #region Content Management - Articles

        /// <summary>
        /// Get all articles for moderation
        /// </summary>
        [HttpGet("articles")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllArticles()
        {
            try
            {
                var articles = await _adminService.GetAllArticlesAsync();
                return Ok(new { articles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving articles");
                return StatusCode(500, new { message = "Failed to retrieve articles" });
            }
        }

        /// <summary>
        /// Approve article
        /// </summary>
        [HttpPost("articles/{articleId}/approve")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveArticle(Guid articleId)
        {
            try
            {
                var result = await _adminService.ApproveArticleAsync(articleId);

                if (!result)
                    return NotFound(new { message = "Article not found" });

                return Ok(new { message = "Article approved" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving article {ArticleId}", articleId);
                return StatusCode(500, new { message = "Failed to approve article" });
            }
        }

        /// <summary>
        /// Reject article
        /// </summary>
        [HttpPost("articles/{articleId}/reject")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectArticle(Guid articleId)
        {
            try
            {
                var result = await _adminService.RejectArticleAsync(articleId);

                if (!result)
                    return NotFound(new { message = "Article not found" });

                return Ok(new { message = "Article rejected" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting article {ArticleId}", articleId);
                return StatusCode(500, new { message = "Failed to reject article" });
            }
        }

        #endregion

        #region Content Management - Jobs

        /// <summary>
        /// Get all jobs for moderation
        /// </summary>
        [HttpGet("jobs")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllJobs()
        {
            try
            {
                var jobs = await _adminService.GetAllJobsAsync();
                return Ok(new { jobs });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs");
                return StatusCode(500, new { message = "Failed to retrieve jobs" });
            }
        }

        /// <summary>
        /// Approve job posting
        /// </summary>
        [HttpPost("jobs/{jobId}/approve")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        #endregion

        #region Content Management - Projects

        /// <summary>
        /// Get all projects for moderation
        /// </summary>
        [HttpGet("projects")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                var projects = await _adminService.GetAllProjectsAsync();
                return Ok(new { projects });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects");
                return StatusCode(500, new { message = "Failed to retrieve projects" });
            }
        }

        /// <summary>
        /// Approve project
        /// </summary>
        [HttpPost("projects/{projectId}/approve")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveProject(int projectId)
        {
            try
            {
                var result = await _adminService.ApproveProjectAsync(projectId);

                if (!result)
                    return NotFound(new { message = "Project not found" });

                return Ok(new { message = "Project approved" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving project {ProjectId}", projectId);
                return StatusCode(500, new { message = "Failed to approve project" });
            }
        }

        /// <summary>
        /// Reject project
        /// </summary>
        [HttpPost("projects/{projectId}/reject")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectProject(int projectId)
        {
            try
            {
                var result = await _adminService.RejectProjectAsync(projectId);

                if (!result)
                    return NotFound(new { message = "Project not found" });

                return Ok(new { message = "Project rejected" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting project {ProjectId}", projectId);
                return StatusCode(500, new { message = "Failed to reject project" });
            }
        }

        #endregion
    }
}
