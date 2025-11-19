using System.ComponentModel.DataAnnotations;

namespace Invert.Api.Dtos.Admin
{
    public class AssignRoleDto
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
