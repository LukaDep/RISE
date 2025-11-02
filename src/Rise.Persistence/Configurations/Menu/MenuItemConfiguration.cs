using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Menu;

namespace Rise.Persistence.Configurations.Menu;

/// <summary>
/// Specific configuration for <see cref="MenuItem"/>.
/// </summary>
internal class MenuItemConfiguration : EntityConfiguration<MenuItem>
{
    public override void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Type)
            .IsRequired();

        // Configure the shadow foreign key property
        builder.Property<string>("MenuId")
            .IsRequired()
            .HasMaxLength(36);
    }
}
