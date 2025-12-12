namespace Rise.Shared.Notifications;

/// <summary>
/// Request wrappers for notification preferences operations.
/// </summary>
public static partial class NotificationPreferencesRequest
{
    /// <summary>
    /// Request to update notification preferences.
    /// Contains flags for each notification category.
    /// </summary>
    public class Edit
    {
        /// <summary>
        /// Notifications for new or updated grades.
        /// </summary>
        public bool GradesNotifications { get; set; }

        /// <summary>
        /// Notifications for schedule or planning changes.
        /// </summary>
        public bool ScheduleNotifications { get; set; }

        /// <summary>
        /// Notifications for campus-related announcements.
        /// </summary>
        public bool CampusNotifications { get; set; }

        /// <summary>
        /// Notifications for news or general updates.
        /// </summary>
        public bool NewsNotifications { get; set; }
    }
}
