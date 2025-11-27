using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.HomeWidgets;

namespace Rise.Persistence.Configurations.Widgets;

internal class WidgetConfiguration : EntityConfiguration<Widget>
{
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