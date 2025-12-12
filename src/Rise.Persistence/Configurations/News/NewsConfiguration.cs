using System;
using Rise.Domain.News;

namespace Rise.Persistence.Configurations.News;

/// <summary>
/// Entity Framework configuration for <see cref="NewsArticle"/>.
/// Configures property constraints for title, description, content, and other fields.
/// </summary>
internal class NewsConfiguration : EntityConfiguration<NewsArticle>
{
    /// <summary>
    /// Configures the NewsArticle entity properties including max lengths and required fields.
    /// </summary>
    /// <param name="builder">The entity type builder for NewsArticle.</param>
    public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<NewsArticle> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PublishDate)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(10000);

        builder.Property(x => x.Author)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);
    }
}
