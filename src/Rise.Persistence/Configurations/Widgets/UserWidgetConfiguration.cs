using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.HomeWidgets;

namespace Rise.Persistence.Configurations.Widgets;

/// <summary>
/// Configuration for <see cref="UserWidget"/>.
/// Configures position and size properties for user dashboard widget layouts.
/// </summary>
internal class UserWidgetConfiguration : EntityConfiguration<UserWidget>
{
    /// <summary>
    /// Configures the UserWidget entity including position (X, Y), dimensions (Width, Height, MinWidth),
    /// and foreign key relationships to Widget and IdentityUser.
    /// </summary>
    /// <param name="builder">The entity type builder for UserWidget.</param>
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