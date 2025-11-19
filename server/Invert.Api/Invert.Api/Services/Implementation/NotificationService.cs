using Invert.Api.Dtos;
using Invert.Api.Dtos.User;
using Invert.Api.Entities;
using Invert.Api.Repositories.Interface;
using Invert.Api.Services.Interface;
using Microsoft.Extensions.Logging;

namespace Invert.Api.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IUnitOfWork unitOfWork,
            ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> CreateNotificationAsync(string userId, string title, string message, string type = "Info", string? link = null)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Type = type,
                    Link = link,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Notification.Add(notification);
                await _unitOfWork.Save();

                _logger.LogInformation("Notification created for user {UserId}: {Title}", userId, title);
                return notification.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool includeRead = true)
        {
            try
            {
                var notifications = await _unitOfWork.Notification.GetUserNotificationsAsync(userId, includeRead);

                return notifications.Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    Link = n.Link
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications for user {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            try
            {
                return await _unitOfWork.Notification.GetUnreadCountAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> MarkAsReadAsync(string userId, int notificationId)
        {
            try
            {
                var notification = await _unitOfWork.Notification.Get(n =>
                    n.Id == notificationId && n.UserId == userId && !n.IsDeleted);

                if (notification == null)
                {
                    _logger.LogWarning("Notification {NotificationId} not found for user {UserId}", notificationId, userId);
                    return false;
                }

                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;

                _unitOfWork.Notification.Update(notification);
                await _unitOfWork.Save();

                _logger.LogInformation("Notification {NotificationId} marked as read for user {UserId}", notificationId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read for user {UserId}",
                    notificationId, userId);
                return false;
            }
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            try
            {
                await _unitOfWork.Notification.MarkAllAsReadAsync(userId);

                _logger.LogInformation("All notifications marked as read for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> DeleteNotificationAsync(string userId, int notificationId)
        {
            try
            {
                var notification = await _unitOfWork.Notification.Get(n =>
                    n.Id == notificationId && n.UserId == userId);

                if (notification == null)
                {
                    _logger.LogWarning("Notification {NotificationId} not found for user {UserId}", notificationId, userId);
                    return false;
                }

                notification.IsDeleted = true;

                _unitOfWork.Notification.Update(notification);
                await _unitOfWork.Save();

                _logger.LogInformation("Notification {NotificationId} deleted for user {UserId}", notificationId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {NotificationId} for user {UserId}",
                    notificationId, userId);
                return false;
            }
        }
    }
}
