using Invert.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invert.Api.Data.Config
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.HasKey(j => j.Id);

            builder.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(j => j.Location)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(j => j.EmploymentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(j => j.ExperienceLevel)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(j => j.Salary)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasMaxLength(50);

            builder.Property(j => j.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(j => j.DatePosted)
                .IsRequired();

            builder.Property(j => j.ClosingDate)
                .IsRequired();

            builder.Property(j => j.Description)
                .IsRequired();

            builder.Property(j => j.Requirements)
                .IsRequired(false);

            builder.Property(j => j.Skills)
                .IsRequired(false);

            builder.Property(j => j.Benefits)
                .IsRequired(false);
        }
    }
}