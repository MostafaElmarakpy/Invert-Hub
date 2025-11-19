namespace Invert.Api.Dtos.Admin
{
    public class UserManagementDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public bool IsLocked { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastLogin { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public int ArticlesCount { get; set; }
        public int JobsCount { get; set; }
        public int ProjectsCount { get; set; }
    }
}
