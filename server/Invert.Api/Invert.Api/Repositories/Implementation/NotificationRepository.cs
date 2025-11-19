using Invert.Api.Data;
using Invert.Api.Entities;
using Invert.Api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Invert.Api.Repositories.Implementation
{
    public class NotificationRepository : GenaricRepository<Notification>, INotificationRepository
    {
        private readonly ApplicationDbContext _db;

        public NotificationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, bool includeRead = true)
        {
            var query = _db.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted);

            if (!includeRead)
            {
                query = query.Where(n => !n.IsRead);
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _db.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _db.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

        public void Update(Notification entity)
        {
            _db.Notifications.Update(entity);
        }
    }
}
