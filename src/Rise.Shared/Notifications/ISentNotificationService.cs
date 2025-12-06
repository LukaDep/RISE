namespace Rise.Shared.Notifications;

/// <summary>
/// Service interface for managing sent notifications.
/// </summary>
public interface ISentNotificationService
{
    /// <summary>
    /// Gets all sent notifications for the current user.
    /// </summary>
    Task<Result<SentNotificationResponse.Index>> GetUserNotificationsAsync(int page = 1, int pageSize = 20, CancellationToken ctx = default);

    /// <summary>
    /// Marks a specific notification as read.
    /// </summary>
    Task<Result> MarkAsReadAsync(SentNotificationRequest.MarkAsRead req, CancellationToken ctx = default);

    /// <summary>
    /// Marks all notifications as read for the current user.
    /// </summary>
    Task<Result> MarkAllAsReadAsync(CancellationToken ctx = default);

    /// <summary>
    /// Gets the count of unread notifications for the current user.
    /// </summary>
    Task<Result<int>> GetUnreadCountAsync(CancellationToken ctx = default);

    /// <summary>
    /// Deletes a specific notification.
    /// </summary>
    Task<Result> DeleteNotificationAsync(Guid notificationId, CancellationToken ctx = default);

    /// <summary>
    /// Saves a notification that was sent to a user.
    /// Called internally when a push notification is sent.
    /// </summary>
    /// <param name="userId">The ID of the user who received the notification.</param>
    /// <param name="title">The notification title.</param>
    /// <param name="body">The notification body.</param>
    /// <param name="url">Optional URL for the notification.</param>
    /// <param name="notificationType">The type/category of the notification.</param>
    /// <param name="deliveryStatus">The delivery status of the push notification.</param>
    /// <param name="ctx">Cancellation token.</param>
    Task SaveSentNotificationAsync(Guid userId, string title, string body, string? url = null, string? notificationType = null, DeliveryStatus deliveryStatus = DeliveryStatus.Pending, CancellationToken ctx = default);
}

/// <summary>
/// Represents the delivery status of a push notification.
/// Defined here for shared access between client and server.
/// </summary>
public enum DeliveryStatus
{
    /// <summary>
    /// Notification is pending delivery (not yet attempted).
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Push notification was successfully delivered.
    /// </summary>
    Delivered = 1,

    /// <summary>
    /// Push notification delivery failed.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// User has no push subscription, notification stored for in-app viewing only.
    /// </summary>
    NoSubscription = 3
}
