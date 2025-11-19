using Invert.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invert.Api.Data.Config
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);
            // Navigation properties
            builder.HasOne<AppUser>(a => a.AppUser)   // navigation property
               .WithMany(u => u.Projects)             // collection on user
               .HasForeignKey(a => a.UserId)
               .HasPrincipalKey(u => u.Id)
               .OnDelete(DeleteBehavior.SetNull);


            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.Description)
                .IsRequired(false);

            builder.Property(p => p.PathImg)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(p => p.Link)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false);
        }
    }
}