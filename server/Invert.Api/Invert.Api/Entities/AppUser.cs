using Microsoft.AspNetCore.Identity;

namespace Invert.Api.Entities
{
    public class AppUser : IdentityUser
    {
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? LastLoginAt { get; set; }

        // ✅ NEW: Cloudinary profile picture properties
        public string? PathImg { get; set; }


        // ✅ NEW: Navigation properties
        public ICollection<Article> Articles { get; set; } = new List<Article>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        // Optional: Additional profile fields
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? Website { get; set; }
    }
}