using System.ComponentModel.DataAnnotations;

namespace Invert.Api.Dtos.User
{
    public class ChangeEmailDto
    {

        [Required(ErrorMessage = "New email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters")]
        public string NewEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;
    }
}
