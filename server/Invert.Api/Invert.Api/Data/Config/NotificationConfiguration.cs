using Invert.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invert.Api.Data.Config
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);
            // Navigation properties
            builder.HasOne<AppUser>(a => a.AppUser)   // navigation property
               .WithMany(u => u.Notifications)             // collection on user
               .HasForeignKey(a => a.UserId)
               .HasPrincipalKey(u => u.Id)
               .OnDelete(DeleteBehavior.SetNull);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(n => n.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

        }
    }
}