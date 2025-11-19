using Invert.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invert.Api.Data.Config
{
    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("Articles");
            //  Navigation properties

            builder.HasOne<AppUser>(a => a.AppUser)   // navigation property
               .WithMany(u => u.Articles)             // collection on user
               .HasForeignKey(a => a.UserId)
               .HasPrincipalKey(u => u.Id)
               .OnDelete(DeleteBehavior.SetNull);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.ContentJson)
                .IsRequired();


            builder.Property(a => a.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(a => a.UpdatedAt)
                .IsRequired(false);

            builder.Property(a => a.Author)
                .IsRequired(false);

            builder.Property(a => a.UserId)
                .IsRequired(false);

        }
    }
}
