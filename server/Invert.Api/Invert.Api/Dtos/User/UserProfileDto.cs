using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Invert.Api.Dtos
{
    public class UserProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

        public string? PathImg { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? Website { get; set; }
            
        public DateTime Created { get; set; }
        public DateTime? LastLogin { get; set; }

        // Statistics
        public int TotalArticles { get; set; }
        public int TotalProjects { get; set; }
        public int TotalNotifications { get; set; }
        public int UnreadNotifications { get; set; }

    }
}
