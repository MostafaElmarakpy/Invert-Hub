using System.ComponentModel.DataAnnotations;

namespace Invert.Api.Dtos
{
    public class RegisterDto
    {

        public required string UserName { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}   