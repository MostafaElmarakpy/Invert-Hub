using System.ComponentModel.DataAnnotations.Schema;

namespace Invert.Api.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        // User this notification belongs to
        public string? UserId { get; set; } = string.Empty;
        public virtual AppUser? AppUser { get; set; }

        // Notification content
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // Notification type (Info, Success, Warning, Error)
        public string Type { get; set; } = "Info";

        // Status
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional: Link to related entity
        public string? Link { get; set; }
        public string? EntityType { get; set; }  // Article, Job, Project
        public string? EntityId { get; set; }
    }
}

