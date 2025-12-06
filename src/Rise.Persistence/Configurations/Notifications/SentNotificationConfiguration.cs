using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Notifications;
using Rise.Shared.Notifications;

namespace Rise.Persistence.Configurations.Notifications;

/// <summary>
/// Entity configuration for <see cref="SentNotification"/>.
/// </summary>
internal class SentNotificationConfiguration : EntityConfiguration<SentNotification>
{
    public override void Configure(EntityTypeBuilder<SentNotification> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.UserId)
            .IsRequired();

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

        builder.HasIndex(x => new { x.UserId, x.IsDeleted, x.SentAt });
    }
}
