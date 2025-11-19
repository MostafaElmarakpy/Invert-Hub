using Invert.Api.Dtos.Article;
using Invert.Api.Dtos.Project;
using Invert.Api.Dtos.User;
using Invert.Api.Entities;
using Invert.Api.Repositories.Interface;

namespace Invert.Api.Services.Interface
{
    public interface INotificationService
    {
        Task<int> CreateNotificationAsync(string userId, string title, string message, string type = "Info", string? link = null);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool includeRead = true);
        Task<bool> MarkAsReadAsync(string userId, int notificationId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<bool> DeleteNotificationAsync(string userId, int notificationId);

    }
}
