using System.ComponentModel.DataAnnotations;

namespace Invert.Api.Dtos.User
{
    public class UpdateProfileDto
    {
        [StringLength(500, ErrorMessage = "Bio must not exceed 500 characters")]
        public string? Bio { get; set; }

        [StringLength(100, ErrorMessage = "Location must not exceed 100 characters")]
        public string? Location { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        [StringLength(200, ErrorMessage = "Website URL must not exceed 200 characters")]
        public string? Website { get; set; }

        [StringLength(100, ErrorMessage = "UserName must not exceed 100 characters")]
        public string? UserName { get; set; }
    }
}
