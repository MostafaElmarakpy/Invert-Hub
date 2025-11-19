using Invert.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invert.Api.Data.Config
{
    public class UserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.Created)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.LastLoginAt)
                .IsRequired(false);
            // Navigation properties
            builder.HasMany(u => u.Articles)
                .WithOne()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.HasMany(u => u.Projects)
                .WithOne()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.HasMany(u => u.Notifications)
                .WithOne()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(u => u.PathImg)
                .IsRequired(false);

            builder.Property(u => u.Bio)
                .IsRequired(false);
            builder.Property(u => u.Location)
                .IsRequired(false);
            builder.Property(u => u.Website)
                .IsRequired(false);


        }
    }
}