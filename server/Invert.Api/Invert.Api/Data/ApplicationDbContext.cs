using Invert.Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Invert.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; } = null!;
        // احذف السطر التالي لأن IdentityDbContext<AppUser> بالفعل يعرّف Users:
        // public DbSet<AppUser> Users { get; set; } = null!;

        public DbSet<Article> Articles { get; set; } = null!;

        public DbSet<Notification> Notifications { get; set; } = null!;

        public DbSet<Job> Jobs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().HasIndex(u => u.UserName).IsUnique();
            modelBuilder.Entity<AppUser>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<AppUser>()
               .HasIndex(u => u.UserName)
               .IsUnique();

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ✅ NEW: Article indexes
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.UserId);




            // ✅ NEW: Project indexes
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.UserId);

            // ✅ NEW: Notification indexes
            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.UserId);

            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.UserId, n.IsRead });

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
