using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.HomeWidgets;

namespace Rise.Persistence.Configurations.Widgets;

/// <summary>
/// Configuration for <see cref="UserWidget"/>.
/// </summary>
internal class UserWidgetConfiguration : EntityConfiguration<UserWidget>
{
    public override void Configure(EntityTypeBuilder<UserWidget> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.X)
            .IsRequired();

        builder.Property(x => x.Y)
            .IsRequired();

        builder.Property(x => x.Width)
            .IsRequired();

        builder.Property(x => x.Height)
            .IsRequired();

        builder.Property(x => x.MinWidth)
            .IsRequired();

        // Configure foreign key relationship to IdentityUser
        builder.HasOne(uw => uw.Widget)
            .WithMany(w => w.UserWidgets)
            .HasForeignKey(uw => uw.WidgetId)   // correct FK property
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(uw => uw.UserId)
            .IsRequired();
    }
}