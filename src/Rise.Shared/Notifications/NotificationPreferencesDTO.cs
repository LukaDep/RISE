namespace Rise.Shared.Notifications;

/// <summary>
/// Data transfer objects for notification preferences.
/// </summary>
public static class NotificationPreferencesDTO
{
    /// <summary>
    /// Represents a user's notification preferences.
    /// Contains flags for each notification category (grades, schedule, campus, news) and overall enabled status.
    /// </summary>
    public class Index
    {
        public Guid UserId { get; set; }
        public bool GradesNotifications { get; set; }
        public bool ScheduleNotifications { get; set; }
        public bool CampusNotifications { get; set; }
        public bool NewsNotifications { get; set; }
        public bool IsEnabled { get; set; }
    }
}

