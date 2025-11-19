using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invert.Api.Entities
{
    public class Article
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        // Store EditorJS JSON as string
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string ContentJson { get; set; } = string.Empty;

        [MaxLength(200)]
        // ✅ NEW: Author tracking
        public string? Author { get; private set; }  // Author name or email
        public string? UserId { get; private set; }  // Link to AppUser
        public virtual AppUser? AppUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }
        public string? Status { get; set; } = "Pending"; // e.g., Pending, Approved, Rejected

        // Constructor for creating new article
        public Article() { }

        public Article(string title, string contentJson, string? author = null, string? userId = null)
        {
            Id = Guid.NewGuid();
            Title = title;
            ContentJson = contentJson;
            Author = author;
            CreatedAt = DateTime.UtcNow;
        }

        // Method to update article
        public void Update(string? title, string? contentJson, string? author)
        {
            if (!string.IsNullOrWhiteSpace(title))
                Title = title;

            if (!string.IsNullOrWhiteSpace(contentJson))
                ContentJson = contentJson;

            if (!string.IsNullOrWhiteSpace(author))
                Author = author;

            UpdatedAt = DateTime.UtcNow;
        }
    }
}
