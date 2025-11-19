namespace Invert.Api.Dtos.User
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Info, Success, Warning, Error
        public bool? IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Link { get; set; }
    }
}
