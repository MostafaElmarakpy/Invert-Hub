using Invert.Api.Entities;
namespace Invert.Api.Repositories.Interface
{
    public interface INotificationRepository : IGenaricRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, bool includeRead = true);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAllAsReadAsync(string userId);

        void Update(Notification entity);
    }
}
