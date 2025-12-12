using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Campus;

namespace Rise.Persistence.Configurations.Campus;

/// <summary>
/// Specific configuration for <see cref="Building"/>.
/// Configures property constraints and shadow foreign key for Campus relationship.
/// </summary>
internal class BuildingConfiguration : EntityConfiguration<Building>
{
    /// <summary>
    /// Configures the Building entity properties including name, address, and shadow CampusId foreign key.
    /// </summary>
    /// <param name="builder">The entity type builder for Building.</param>
    public override void Configure(EntityTypeBuilder<Building> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.BuildingCode)
            .IsRequired()
            .HasMaxLength(5);

        // Configure the shadow foreign key property
        builder.Property<Guid>("CampusId")
            .IsRequired()
            .HasMaxLength(36);
    }
}
