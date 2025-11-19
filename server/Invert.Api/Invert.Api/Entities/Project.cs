using System.ComponentModel.DataAnnotations.Schema;

namespace Invert.Api.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? PathImg { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        // Note: Add Status property to Project entity if needed
        public string? Status { get; set; } = "Pending"; // e.g., Pending, Approved, Rejected
        public DateTime? ApprovedAt { get; set; }

        // ✅ NEW: Track project owner
        public string? Owner { get; set; }      // Owner name
        public string? UserId { get; set; }     // Link to AppUser
        public virtual AppUser? AppUser { get; set; }

    }
}
