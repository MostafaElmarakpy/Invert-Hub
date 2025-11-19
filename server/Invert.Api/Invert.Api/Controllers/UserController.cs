using Invert.Api.Dtos;
using Invert.Api.Dtos.User;
using Invert.Api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Invert.Api.Controllers
{
    /// <summary>
    /// User profile and activity management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // All endpoints require authentication
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Helper to get current user ID from token
        /// </summary>
        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User not authenticated");
        }

        #region Profile Management

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var profile = await _userService.GetProfileAsync(userId);

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(500, new { message = "Failed to retrieve profile" });
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("profile")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                var result = await _userService.UpdateProfileAsync(userId, dto);

                if (!result)
                    return BadRequest(new { message = "Failed to update profile" });

                _logger.LogInformation("Profile updated for user {UserId}", userId);
                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return StatusCode(500, new { message = "Failed to update profile" });
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [HttpPost("change-password")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                var result = await _userService.ChangePasswordAsync(userId, dto);

                if (!result)
                    return BadRequest(new { message = "Failed to change password" });

                _logger.LogInformation("Password changed for user {UserId}", userId);
                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, new { message = "Failed to change password" });
            }
        }

        /// <summary>
        /// Change user Username
        /// </summary>
         [HttpPost("change-username")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeUsername([FromBody] UpdateUsernameDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                var result = await _userService.ChangeUsernameAsync(userId, dto);

                if (!result)
                    return BadRequest(new { message = "Failed to change username" });

                _logger.LogInformation("Username changed for user {UserId}", userId);
                return Ok(new { message = "Username changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing username");
                return StatusCode(500, new { message = "Failed to change username" });
            }
        }



        /// <summary>
        /// Change user email
        /// </summary>
        //[HttpPost("change-email")]
        //// [ProducesResponseType(StatusCodes.Status200OK)]
        //// [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto dto)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        var userId = GetCurrentUserId();
        //        var result = await _userService.ChangeEmailAsync(userId, dto.NewEmail);

        //        if (!result)
        //            return BadRequest(new { message = "Failed to change email" });

        //        _logger.LogInformation("Email changed for user {UserId}", userId);
        //        return Ok(new { message = "Email changed successfully. Please verify your new email." });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error changing email");
        //        return StatusCode(500, new { message = "Failed to change email" });
        //    }
        //}

        /// <summary>
        /// Upload profile picture
        /// </summary>
        [HttpPost("profile-picture")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "No file uploaded" });

                // Validate file type and size
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                    return BadRequest(new { message = "Invalid file type. Only JPG, PNG, and GIF are allowed." });

                if (file.Length > 5 * 1024 * 1024) // 5MB
                    return BadRequest(new { message = "File size must be less than 5MB" });

                var userId = GetCurrentUserId();
                //var result = await _userService.UpdateProfileAsync(userId, file);

                //if (!result)
                //    return BadRequest(new { message = "Failed to upload profile picture" });

                _logger.LogInformation("Profile picture uploaded for user {UserId}", userId);
                return Ok(new { message = "Profile picture uploaded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture");
                return StatusCode(500, new { message = "Failed to upload profile picture" });
            }
        }

        #endregion

        #region User Content

        /// <summary>
        /// Get user's articles
        /// <summary>
        /// Get user's articles
        /// </summary>
        /// </summary>
        [HttpGet("my-articles")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyArticles()
        {
            try
            {
                var userId = GetCurrentUserId();
                var articles = await _userService.GetMyArticlesAsync(userId);

                return Ok(new { articles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user articles");
                return StatusCode(500, new { message = "Failed to retrieve articles" });
            }
        }

        /// <summary>
        /// Get user's projects
        /// </summary>
        [HttpGet("my-projects")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyProjects()
        {
            try
            {
                var userId = GetCurrentUserId();
                var projects = await _userService.GetMyProjectsAsync(userId);

                return Ok(new { projects });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user projects");
                return StatusCode(500, new { message = "Failed to retrieve projects" });
            }
        }

        #endregion

        #region Activity & Notifications

        /// <summary>
        /// Get user activity summary
        /// </summary>
        //[HttpGet("activity")]
        //// [ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetActivity()
        //{
        //    try
        //    {
        //        var userId = GetCurrentUserId();
        //        var activity = await _userService.GetActivityAsync(userId);

        //        return Ok(activity);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error retrieving user activity");
        //        return StatusCode(500, new { message = "Failed to retrieve activity" });
        //    }
        //}

        /// <summary>
        /// Get user notifications
        /// </summary>
        [HttpGet("notifications")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                var notifications = await _userService.GetNotificationsAsync(userId);

                return Ok(new { notifications });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications");
                return StatusCode(500, new { message = "Failed to retrieve notifications" });
            }
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        [HttpPost("notifications/{notificationId}/read")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _userService.MarkNotificationAsReadAsync(userId, notificationId);

                if (!result)
                    return NotFound(new { message = "Notification not found" });

                return Ok(new { message = "Notification marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return StatusCode(500, new { message = "Failed to mark notification as read" });
            }
        }


        #region Profile Picture - Cloudinary

        /// <summary>
        /// Upload profile picture from file (Flow 1)
        ///// </summary>
        //[HttpPost("profile-picture/upload")]
        //// [ProducesResponseType(typeof(ProfilePictureResponseDto), StatusCodes.Status200OK)]
        //// [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //// [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<IActionResult> UploadProfilePictureFromFile([FromForm] IFormFile file)
        //{
        //    try
        //    {
        //        // Validation is done in controller for early return
        //        if (file == null || file.Length == 0)
        //        {
        //            return BadRequest(new { message = "No file uploaded" });
        //        }

        //        var userId = GetCurrentUserId();
        //        var result = await _userService.UploadProfilePictureFromFileAsync(userId, file);

        //        if (!result.Success)
        //        {
        //            _logger.LogWarning("Profile picture upload failed for user {UserId}: {Message}",
        //                userId, result.Message);
        //            return BadRequest(new { message = result.Message });
        //        }

        //        _logger.LogInformation("Profile picture uploaded successfully for user {UserId}", userId);

        //        return Ok(new
        //        {
        //            message = result.Message,
        //            profilePictureUrl = result.ProfilePictureUrl,
        //            profilePicturePublicId = result.ProfilePicturePublicId
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error uploading profile picture");
        //        return StatusCode(500, new { message = "Failed to upload profile picture" });
        //    }
        //}

        /// <summary>
        /// Set profile picture from existing Cloudinary image (Flow 2)
        /// </summary>
        //[HttpPost("profile-picture/set-from-cloudinary")]
        //// [ProducesResponseType(StatusCodes.Status200OK)]
        //// [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //// [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<IActionResult> SetProfilePictureFromCloudinary([FromBody] CloudinaryImageDto dto)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var userId = GetCurrentUserId();
        //        var result = await _userService.SetProfilePictureFromCloudinaryAsync(userId, dto);

        //        if (!result)
        //        {
        //            _logger.LogWarning("Failed to set profile picture from Cloudinary for user {UserId}", userId);
        //            return BadRequest(new { message = "Failed to set profile picture from Cloudinary" });
        //        }

        //        _logger.LogInformation("Profile picture set from Cloudinary for user {UserId}", userId);

        //        return Ok(new
        //        {
        //            message = "Profile picture updated successfully",
        //            profilePictureUrl = dto.Url,
        //            profilePicturePublicId = dto.PublicId
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error setting profile picture from Cloudinary");
        //        return StatusCode(500, new { message = "Failed to set profile picture" });
        //    }
        //}

        /// <summary>
        /// Delete profile picture
        /// </summary>
        //[HttpDelete("profile-picture")]
        //// [ProducesResponseType(StatusCodes.Status200OK)]
        //// [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<IActionResult> DeleteProfilePicture()
        //{
        //    try
        //    {
        //        var userId = GetCurrentUserId();
        //        var result = await _userService.DeleteProfilePictureAsync(userId);
        //        //var result = await _userService.DeleteProfilePictureAsync(userId);

        //        if (!result)
        //        {
        //            _logger.LogWarning("Failed to delete profile picture for user {UserId}", userId);
        //            return BadRequest(new { message = "Failed to delete profile picture" });
        //        }

        //        _logger.LogInformation("Profile picture deleted for user {UserId}", userId);
        //        return Ok(new { message = "Profile picture deleted successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error deleting profile picture");
        //        return StatusCode(500, new { message = "Failed to delete profile picture" });
        //    }
        //}

        /// <summary>
        /// Get current profile picture info
        /// </summary>
        [HttpGet("profile-picture")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProfilePicture()
        {
            try
            {
                var userId = GetCurrentUserId();
                var profile = await _userService.GetProfileAsync(userId);

                return Ok(new
                {
                    //profile = profile,
                    PathImg = profile.PathImg,

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile picture");
                return StatusCode(500, new { message = "Failed to get profile picture" });
            }
        }

        #endregion

        #endregion
    }
}
