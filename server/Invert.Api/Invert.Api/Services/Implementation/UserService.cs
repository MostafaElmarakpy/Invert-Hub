using AutoMapper;

using Invert.Api.Dtos;
using Invert.Api.Dtos.Article;
using Invert.Api.Dtos.Job;
using Invert.Api.Dtos.Project;
using Invert.Api.Dtos.User;
using Invert.Api.Entities;
using Invert.Api.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Invert.Api.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        //private readonly Cloudinary _cloudinary;
        //private readonly CloudinarySettings _cloudinarySettings;

        public UserService(
            UserManager<AppUser> userManager,
            ILogger<UserService> logger,
            IMapper mapper
            //Cloudinary cloudinary
            //IOptions<CloudinarySettings> cloudinarySettings
            )
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            //_cloudinary = cloudinary;
            //_cloudinarySettings = cloudinarySettings.Value;
        }

        #region Profile Picture - Cloudinary Integration

        /// <summary>
        /// Upload profile picture from IFormFile to Cloudinary
        /// </summary>
        // public async Task<ProfilePictureResponseDto> UploadProfilePictureFromFileAsync(string userId, IFormFile file)
        // {
        //     try
        //     {
        //         // 1. Validate file
        //         var validationError = ValidateImageFile(file);
        //         if (validationError != null)
        //         {
        //             _logger.LogWarning("Profile picture validation failed for user {UserId}: {Error}", userId, validationError);
        //             return new ProfilePictureResponseDto
        //             {
        //                 Success = false,
        //                 Message = validationError
        //             };
        //         }

        //         // 2. Get user
        //         var user = await _userManager.FindByIdAsync(userId);
        //         if (user == null)
        //         {
        //             _logger.LogWarning("User {UserId} not found", userId);
        //             return new ProfilePictureResponseDto
        //             {
        //                 Success = false,
        //                 Message = "User not found"
        //             };
        //         }

        //         // 3. Delete old profile picture if exists
        //         if (!string.IsNullOrEmpty(user.ProfilePicturePublicId))
        //         {
        //             await DeleteFromCloudinaryAsync(user.ProfilePicturePublicId);
        //         }

        //         // 4. Upload to Cloudinary
        //         //var uploadResult = await UploadToCloudinaryAsync(file, userId);

        //         //if (uploadResult == null || uploadResult.Error != null)
        //         //{
        //         //    _logger.LogError("Cloudinary upload failed for user {UserId}: {Error}",
        //         //        userId, uploadResult?.Error?.Message);

        //         //    return new ProfilePictureResponseDto
        //         //    {
        //         //        Success = false,
        //         //        Message = "Failed to upload image to Cloudinary"
        //         //    };
        //         //}

        //         // 5. Update user entity
        //         //user.ProfilePictureUrl = uploadResult.SecureUrl.ToString();
        //         //user.ProfilePicturePublicId = uploadResult.PublicId;

        //         var updateResult = await _userManager.UpdateAsync(user);

        //         if (!updateResult.Succeeded)
        //         {
        //             _logger.LogError("Failed to update user {UserId} with new profile picture", userId);

        //             // Cleanup: delete uploaded image since we couldn't save to DB
        //             //await DeleteFromCloudinaryAsync(uploadResult.PublicId);

        //             return new ProfilePictureResponseDto
        //             {
        //                 Success = false,
        //                 Message = "Failed to save profile picture"
        //             };
        //         }

        //         //_logger.LogInformation("Profile picture uploaded successfully for user {UserId}. PublicId: {PublicId}",
        //         //    userId, uploadResult.PublicId);

        //         return new ProfilePictureResponseDto
        //         {
        //             Success = true,
        //             ProfilePictureUrl = user.ProfilePictureUrl,
        //             ProfilePicturePublicId = user.ProfilePicturePublicId,
        //             Message = "Profile picture uploaded successfully"
        //         };
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error uploading profile picture for user {UserId}", userId);
        //         return new ProfilePictureResponseDto
        //         {
        //             Success = false,
        //             Message = "An error occurred while uploading profile picture"
        //         };
        //     }
        // }

        /// <summary>
        /// Set profile picture from existing Cloudinary image
        /// </summary>
        // public async Task<bool> SetProfilePictureFromCloudinaryAsync(string userId, CloudinaryImageDto image)
        // {
        //     try
        //     {
        //         // 1. Validate input
        //         if (string.IsNullOrWhiteSpace(image.PublicId) || string.IsNullOrWhiteSpace(image.Url))
        //         {
        //             _logger.LogWarning("Invalid Cloudinary image data for user {UserId}", userId);
        //             return false;
        //         }

        //         // 2. Get user
        //         var user = await _userManager.FindByIdAsync(userId);
        //         if (user == null)
        //         {
        //             _logger.LogWarning("User {UserId} not found", userId);
        //             return false;
        //         }

        //         // 3. Verify image exists on Cloudinary (optional but recommended)
        //         var verifyResult = await VerifyCloudinaryImageAsync(image.PublicId);
        //         if (!verifyResult)
        //         {
        //             _logger.LogWarning("Cloudinary image {PublicId} not found or invalid", image.PublicId);
        //             return false;
        //         }

        //         // 4. Delete old profile picture if exists
        //         if (!string.IsNullOrEmpty(user.ProfilePicturePublicId))
        //         {
        //             await DeleteFromCloudinaryAsync(user.ProfilePicturePublicId);
        //         }

        //         // 5. Update user entity
        //         user.ProfilePictureUrl = image.Url;
        //         user.ProfilePicturePublicId = image.PublicId;

        //         var updateResult = await _userManager.UpdateAsync(user);

        //         if (!updateResult.Succeeded)
        //         {
        //             _logger.LogError("Failed to update user {UserId} with Cloudinary image", userId);
        //             return false;
        //         }

        //         _logger.LogInformation("Profile picture set from Cloudinary for user {UserId}. PublicId: {PublicId}",
        //             userId, image.PublicId);

        //         return true;
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error setting profile picture from Cloudinary for user {UserId}", userId);
        //         return false;
        //     }
        // }

        /// <summary>
        /// Delete profile picture
        /// </summary>
        // public async Task<bool> DeleteProfilePictureAsync(string userId)
        // {
        //     try
        //     {
        //         var user = await _userManager.FindByIdAsync(userId);
        //         if (user == null)
        //         {
        //             _logger.LogWarning("User {UserId} not found", userId);
        //             return false;
        //         }

        //         if (string.IsNullOrEmpty(user.ProfilePicturePublicId))
        //         {
        //             _logger.LogInformation("User {UserId} has no profile picture to delete", userId);
        //             return true; // Nothing to delete
        //         }

        //         // Delete from Cloudinary
        //         await DeleteFromCloudinaryAsync(user.ProfilePicturePublicId);

        //         // Clear from user entity
        //         user.ProfilePictureUrl = null;
        //         user.ProfilePicturePublicId = null;

        //         var updateResult = await _userManager.UpdateAsync(user);

        //         if (!updateResult.Succeeded)
        //         {
        //             _logger.LogError("Failed to clear profile picture for user {UserId}", userId);
        //             return false;
        //         }

        //         _logger.LogInformation("Profile picture deleted for user {UserId}", userId);
        //         return true;
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error deleting profile picture for user {UserId}", userId);
        //         return false;
        //     }
        // }

        #endregion

        #region Cloudinary Helper Methods

        /// <summary>
        /// Validate image file before upload
        /// </summary>
        //private string? ValidateImageFile(IFormFile file)
        //{
        //    // Check if file is null or empty
        //    if (file == null || file.Length == 0)
        //    {
        //        return "No file uploaded";
        //    }

        //    // Validate file size (max 5MB)
        //    const long maxFileSize = 5 * 1024 * 1024; // 5MB
        //    if (file.Length > maxFileSize)
        //    {
        //        return "File size must be less than 5MB";
        //    }

        //    // Validate file extension
        //    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        //    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        //    if (!allowedExtensions.Contains(extension))
        //    {
        //        return "Invalid file type. Only JPG, PNG, GIF, and WEBP are allowed";
        //    }

        //    // Validate MIME type
        //    var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        //    if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
        //    {
        //        return "Invalid file format";
        //    }

        //    return null; // Validation passed
        //}

        /// <summary>
        /// Upload image to Cloudinary
        /// </summary>
        //private async Task<ImageUploadResult?> UploadToCloudinaryAsync(IFormFile file, string userId)
        //{
        //    try
        //    {
        //        using var stream = file.OpenReadStream();

        //        var uploadParams = new ImageUploadParams
        //        {
        //            File = new FileDescription(file.FileName, stream),
        //            Folder = _cloudinarySettings.ProfilePicturesFolder,
        //            PublicId = $"user_{userId}_{Guid.NewGuid()}", // Unique public ID
        //            Transformation = new Transformation()
        //                .Width(500)
        //                .Height(500)
        //                .Crop("fill")
        //                .Gravity("face") // Focus on face if detected
        //                .Quality("auto:good"),
        //            Overwrite = false,
        //            UniqueFilename = true
        //        };

        //        var result = await _cloudinary.UploadAsync(uploadParams);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Cloudinary upload exception for user {UserId}", userId);
        //        return null;
        //    }
        //}

        /// <summary>
        /// Delete image from Cloudinary
        /// </summary>
        //private async Task<bool> DeleteFromCloudinaryAsync(string publicId)
        //{
        //    try
        //    {
        //        var deleteParams = new DeletionParams(publicId);
        //        var result = await _cloudinary.DestroyAsync(deleteParams);

        //        if (result.Result == "ok" || result.Result == "not found")
        //        {
        //            _logger.LogInformation("Cloudinary image deleted: {PublicId}", publicId);
        //            return true;
        //        }

        //        _logger.LogWarning("Failed to delete Cloudinary image {PublicId}: {Result}", publicId, result.Result);
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error deleting Cloudinary image {PublicId}", publicId);
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Verify that Cloudinary image exists
        ///// </summary>
        //private async Task<bool> VerifyCloudinaryImageAsync(string publicId)
        //{
        //    try
        //    {
        //        var result = await _cloudinary.GetResourceAsync(publicId);
        //        return result != null && result.StatusCode == System.Net.HttpStatusCode.OK;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        #endregion

        #region Other IUserService Methods (Placeholder implementations)

        public async Task<UserProfileDto> GetProfileAsync(string userId)
        {
            // GetProfile using _userManager and map to UserProfileDto
            try
            {
                var user = await _userManager.Users
                    .Include(u => u.Articles)
                    .Include(u => u.Projects)
                    .Include(u => u.Notifications)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    throw new KeyNotFoundException($"User with ID {userId} not found");
                }

                var profileDto = _mapper.Map<UserProfileDto>(user);

                // Get roles manually (not part of mapping)
                var roles = await _userManager.GetRolesAsync(user);
                profileDto.Roles = roles.ToList();

                return profileDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile for user {UserId}", userId);
                throw;
            }
        }


        public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return false;
                }

                // Update Bio, Location, Website
                if (!string.IsNullOrWhiteSpace(dto.Bio))
                    user.Bio = dto.Bio;

                if (!string.IsNullOrWhiteSpace(dto.Location))
                    user.Location = dto.Location;

                if (!string.IsNullOrWhiteSpace(dto.Website))
                    user.Website = dto.Website;

                // Update UserName if provided
                if (!string.IsNullOrWhiteSpace(dto.UserName) && dto.UserName != user.UserName)
                {
                    // Check if username already exists
                    var existingUser = await _userManager.FindByNameAsync(dto.UserName);
                    if (existingUser != null && existingUser.Id != userId)
                    {
                        _logger.LogWarning("Username {UserName} already exists", dto.UserName);
                        return false;
                    }

                    var setUserNameResult = await _userManager.SetUserNameAsync(user, dto.UserName);
                    if (!setUserNameResult.Succeeded)
                    {
                        _logger.LogError("Failed to update username for user {UserId}: {Errors}",
                            userId, string.Join(", ", setUserNameResult.Errors.Select(e => e.Description)));
                        return false;
                    }
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Profile updated successfully for user {UserId}", userId);
                    return true;
                }

                _logger.LogError("Failed to update profile for user {UserId}: {Errors}",
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
                return false;
            }
        }


        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return false;
                }

                var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Password changed for user {UserId}", userId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to change password for user {UserId}", userId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ChangeUsernameAsync(string userId, UpdateUsernameDto dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return false;
                }

                // Verify password
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.CurrentPassword);
                if (!isPasswordValid)
                {
                    _logger.LogWarning("Invalid password for username change for user {UserId}", userId);
                    return false;
                }

                // Check if username already exists
                var existingUser = await _userManager.FindByNameAsync(dto.NewUserName);
                if (existingUser != null && existingUser.Id != userId)
                {
                    _logger.LogWarning("Username {UserName} already exists", dto.NewUserName);
                    return false;
                }

                // Update username
                var result = await _userManager.SetUserNameAsync(user, dto.NewUserName);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Username changed successfully for user {UserId} to {NewUserName}",
                        userId, dto.NewUserName);
                    return true;
                }

                _logger.LogError("Failed to change username for user {UserId}: {Errors}",
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing username for user {UserId}", userId);
                return false;
            }
        }

        public async Task<IEnumerable<ArticleDto>> GetMyArticlesAsync(string userId)
        {
            try
            {
                var user = await _userManager.Users
                    .Include(u => u.Projects)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    throw new KeyNotFoundException($"User with ID {userId} not found");
                }

                return user.Articles.Select(
                    article => new ArticleDto
                    {
                        Id = article.Id,
                        CreatedAt = article.CreatedAt,
                        UpdatedAt = article.UpdatedAt,
                        Title = article.Title,
                        Author = article.Author,
                        Version = "2.28.2",
                        Blocks = new List<BlockDto>(),
                        ContentJson = article.ContentJson,
                        Time = new DateTimeOffset(article.CreatedAt).ToUnixTimeMilliseconds()
                    })
                    .ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectDto>> GetMyProjectsAsync(string userId)
        {
            try
            {
                var user = await _userManager.Users
                    .Include(u => u.Projects)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    throw new KeyNotFoundException($"User with ID {userId} not found");
                }

                return user.Projects.Select(project => _mapper.Map<ProjectDto>(project)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    throw new KeyNotFoundException($"User with ID {userId} not found");
                }

                // Assuming user.Notifications is a collection of Notification entities
                var notifications = user.Notifications;

                return notifications.Select(notification => _mapper.Map<NotificationDto>(notification)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> MarkNotificationAsReadAsync(string userId, int notificationId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return false;
                }

                var notification = user.Notifications.FirstOrDefault(n => n.Id == notificationId);

                if (notification == null)
                {
                    _logger.LogWarning("Notification {NotificationId} not found for user {UserId}", notificationId, userId);
                    return false;
                }

                notification.IsRead = true;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to mark notification {NotificationId} as read for user {UserId}: {Errors}",
                        notificationId, userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return false;
                }

                _logger.LogInformation("Notification {NotificationId} marked as read for user {UserId}", notificationId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read for user {UserId}", notificationId, userId);
                return false;
            }

        }

        public Task<bool> ChangeUsernameAsync(string userId, string newUsername)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
