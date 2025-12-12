using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.HomeWidgets;

namespace Rise.Persistence.Configurations.Widgets;

/// <summary>
/// Entity Framework configuration for <see cref="Widget"/>.
/// Configures the one-to-many relationship with UserWidget.
/// </summary>
internal class WidgetConfiguration : EntityConfiguration<Widget>
{
    /// <summary>
    /// Configures the Widget entity including TypeName property and one-to-many relationship with UserWidgets.
    /// </summary>
    /// <param name="builder">The entity type builder for Widget.</param>
    public override void Configure(EntityTypeBuilder<Widget> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.TypeName)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasMany(w => w.UserWidgets)
            .WithOne(uw => uw.Widget)
            .HasForeignKey(uw => uw.WidgetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}