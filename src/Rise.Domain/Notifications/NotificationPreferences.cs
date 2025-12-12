namespace Rise.Domain.Notifications;

/// <summary>
/// Represents a user's notification preferences.
/// Contains flags for each notification category (grades, schedule, campus, news).
/// </summary>
public class NotificationPreferences : Entity
{
    /// <summary>
    /// Creates a new NotificationPreferences instance with the specified user ID.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    public NotificationPreferences(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Whether the user wants to receive grade-related notifications.
    /// </summary>
    public bool GradesNotifications { get; set; } = true;

    /// <summary>
    /// Whether the user wants to receive schedule-related notifications.
    /// </summary>
    public bool ScheduleNotifications { get; set; } = true;

    /// <summary>
    /// Whether the user wants to receive campus-related notifications.
    /// </summary>
    public bool CampusNotifications { get; set; } = true;

    /// <summary>
    /// Whether the user wants to receive news notifications.
    /// </summary>
    public bool NewsNotifications { get; set; } = true;

}
