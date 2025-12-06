namespace Rise.Shared.Notifications;

/// <summary>
/// DTOs for sent notifications.
/// </summary>
public static class SentNotificationDTO
{
    /// <summary>
    /// DTO for displaying a sent notification.
    /// </summary>
    public class Index
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Body { get; set; }
        public string? Url { get; set; }
        public string? NotificationType { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}

/// <summary>
/// Response wrapper for sent notifications.
/// </summary>
public static class SentNotificationResponse
{
    /// <summary>
    /// Response containing a list of sent notifications.
    /// </summary>
    public class Index
    {
        public required IEnumerable<SentNotificationDTO.Index> Notifications { get; set; }
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
    }
}

/// <summary>
/// Request DTOs for sent notifications.
/// </summary>
public static class SentNotificationRequest
{
    /// <summary>
    /// Request to mark a notification as read.
    /// </summary>
    public class MarkAsRead
    {
        public Guid NotificationId { get; set; }

        public class Validator : AbstractValidator<MarkAsRead>
        {
            public Validator()
            {
                RuleFor(x => x.NotificationId)
                    .NotEmpty()
                    .WithMessage("Notificatie ID is verplicht.");
            }
        }
    }

    /// <summary>
    /// Request to mark all notifications as read.
    /// </summary>
    public class MarkAllAsRead
    {
        // No properties needed - uses authenticated user
    }
}
