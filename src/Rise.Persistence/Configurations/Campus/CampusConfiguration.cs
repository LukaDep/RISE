using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Campus;

namespace Rise.Persistence.Configurations.Campus;

/// <summary>
/// Specific configuration for <see cref="Domain.Campus.Campus"/>.
/// </summary>
internal class CampusConfiguration : EntityConfiguration<Domain.Campus.Campus>
{
    public override void Configure(EntityTypeBuilder<Domain.Campus.Campus> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Street)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.HouseNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.MapImageUrl)
            .HasMaxLength(500);

        // Configure one-to-many relationship with Building
        builder.HasMany(x => x.Buildings)
            .WithOne()
            .HasForeignKey("CampusId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
