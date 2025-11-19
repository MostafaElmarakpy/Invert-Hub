namespace Invert.Api.Dtos.Admin
{
    public class AdminStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int LockedUsers { get; set; }
        public int TotalArticles { get; set; }
        public int PendingArticles { get; set; }
        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int TotalProjects { get; set; }
        public int PendingProjects { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int NewContentThisMonth { get; set; }
    }
}
