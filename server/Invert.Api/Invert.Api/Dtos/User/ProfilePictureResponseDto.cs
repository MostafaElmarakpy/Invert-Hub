using System.ComponentModel.DataAnnotations;

namespace Invert.Api.Dtos.User
{
    public class ProfilePictureResponseDto
    {
     public bool Success { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? ProfilePicturePublicId { get; set; }
    public string? Message { get; set; }
    }
}
