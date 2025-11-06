using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Campus;

namespace Rise.Persistence.Configurations.Campus;

/// <summary>
/// Specific configuration for <see cref="Building"/>.
/// </summary>
internal class BuildingConfiguration : EntityConfiguration<Building>
{
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

        // Configure the shadow foreign key property
        builder.Property<Guid>("CampusId")
            .IsRequired()
            .HasMaxLength(36);
    }
}
