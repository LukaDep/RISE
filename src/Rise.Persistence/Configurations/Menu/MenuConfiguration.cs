using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Menu;

namespace Rise.Persistence.Configurations.Menu;

/// <summary>
/// Specific configuration for <see cref="Domain.Menu.Menu"/>.
/// Configures the one-to-many relationship with MenuItems.
/// </summary>
internal class MenuConfiguration : EntityConfiguration<Domain.Menu.Menu>
{
    /// <summary>
    /// Configures the Menu entity properties and one-to-many relationship with MenuItems using cascade delete.
    /// </summary>
    /// <param name="builder">The entity type builder for Menu.</param>
    public override void Configure(EntityTypeBuilder<Domain.Menu.Menu> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.RestoId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(x => x.Date)
            .IsRequired();

        // Configure one-to-many relationship with MenuItem
        builder.HasMany(x => x.MenuItems)
            .WithOne()
            .HasForeignKey("MenuId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
