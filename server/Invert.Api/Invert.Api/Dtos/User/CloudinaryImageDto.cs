using System.ComponentModel.DataAnnotations;

namespace Invert.Api.Dtos.User
{
    public class CloudinaryImageDto
    {
        [Required]
        public string PublicId { get; set; } = string.Empty;

        /// <summary>
        /// Cloudinary secure URL
        /// </summary>
        [Required]
        [Url]
        public string Url { get; set; } = string.Empty;
    }
}
