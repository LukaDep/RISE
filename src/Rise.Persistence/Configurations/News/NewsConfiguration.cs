using System;
using Rise.Domain.News;

namespace Rise.Persistence.Configurations.News;

internal class NewsConfiguration : EntityConfiguration<NewsArticle>
{
    public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<NewsArticle> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .IsRequired()
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
    }
}
