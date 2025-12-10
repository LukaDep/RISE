using Rise.Shared.Notifications;

namespace Rise.Domain.Notifications;

/// <summary>
/// Represents a notification that has been sent to a user.
/// </summary>
public class SentNotification : Entity
{
    /// <summary>
    /// The ID of the push subscription this notification was sent to.
    /// </summary>
    public Guid PushSubscriptionId { get; set; }

    /// <summary>
    /// Navigation property to the push subscription.
    /// </summary>
    public PushSubscriptions? PushSubscription { get; set; }

    /// <summary>
    /// The ID of the user who received the notification (nullable for anonymous users).
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// The title of the notification.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// The body/content of the notification.
    /// </summary>
    public required string Body { get; set; }

    /// <summary>
    /// Optional URL that the notification links to.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// The type/category of the notification (e.g., "grades", "schedule", "campus", "news").
    /// </summary>
    public string? NotificationType { get; set; }

    /// <summary>
    /// The date and time when the notification was sent.
    /// </summary>
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether the notification has been read by the user.
    /// </summary>
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// The date and time when the notification was read (if applicable).
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// The delivery status of the push notification.
    /// Indicates whether the push notification was successfully delivered.
    /// </summary>
    public DeliveryStatus DeliveryStatus { get; set; } = DeliveryStatus.Pending;
}
