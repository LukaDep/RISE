using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Notifications;
using Rise.Shared.Notifications;

namespace Rise.Persistence.Configurations.Notifications;

/// <summary>
/// Entity configuration for <see cref="SentNotification"/>.
/// Configures relationships with PushSubscriptions and property constraints for notification tracking.
/// </summary>
internal class SentNotificationConfiguration : EntityConfiguration<SentNotification>
{
    /// <summary>
    /// Configures the SentNotification entity including foreign key to PushSubscriptions with cascade delete,
    /// and properties for title, body, URL, and delivery status tracking.
    /// </summary>
    /// <param name="builder">The entity type builder for SentNotification.</param>
    public override void Configure(EntityTypeBuilder<SentNotification> builder)
    {
        base.Configure(builder);

        // Foreign key to PushSubscriptions
        builder.Property(x => x.PushSubscriptionId)
            .IsRequired();

        builder.HasOne(x => x.PushSubscription)
            .WithMany()
            .HasForeignKey(x => x.PushSubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserId is nullable (for anonymous users)
        builder.Property(x => x.UserId)
            .IsRequired(false);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Body)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Url)
            .HasMaxLength(500);

        builder.Property(x => x.NotificationType)
            .HasMaxLength(50);

        builder.Property(x => x.SentAt)
            .IsRequired();

        builder.Property(x => x.IsRead)
            .HasDefaultValue(false);

        builder.Property(x => x.DeliveryStatus)
            .HasDefaultValue(DeliveryStatus.Pending);

        // Index on PushSubscriptionId for faster lookups
        builder.HasIndex(x => x.PushSubscriptionId);

        // Index on UserId for queries that still filter by user
        builder.HasIndex(x => new { x.UserId, x.IsDeleted, x.SentAt });
    }
}
