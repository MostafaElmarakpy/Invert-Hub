using Invert.Api.Dtos;
using Invert.Api.Dtos.Article;
using Invert.Api.Dtos.Job;
using Invert.Api.Dtos.Project;
using Invert.Api.Dtos.User;
using Invert.Api.Entities;

namespace Invert.Api.Services.Interface
{
    public interface IUserService
    {
        // Profile Management
        Task<UserProfileDto> GetProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<bool> ChangeUsernameAsync(string userId, UpdateUsernameDto dto);
        //Task<bool> ChangeEmailAsync(string userId, string newEmail);

        // ✅ NEW: Profile Picture Methods
        /// <summary>
        /// Upload profile picture from file and store on Cloudinary
        /// </summary>
        // Task<ProfilePictureResponseDto> UploadProfilePictureFromFileAsync(string userId, IFormFile file);

        /// <summary>
        /// Set profile picture from existing Cloudinary image
        /// </summary>
        // Task<bool> SetProfilePictureFromCloudinaryAsync(string userId, CloudinaryImageDto image);

        /// <summary>
        /// Delete profile picture from Cloudinary and user record
        /// </summary>
        // Task<bool> DeleteProfilePictureAsync(string userId);

        // User Content
        Task<IEnumerable<ArticleDto>> GetMyArticlesAsync(string userId);
        Task<IEnumerable<ProjectDto>> GetMyProjectsAsync(string userId);
        // Activity
        Task<IEnumerable<NotificationDto>> GetNotificationsAsync(string userId);
        Task<bool> MarkNotificationAsReadAsync(string userId, int notificationId);

    }
}
