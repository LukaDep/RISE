namespace Rise.Shared.Notifications;

public static class NotificationPreferencesDTO
{
    public class Index
    {
        public Guid UserId { get; set; }
        public bool GradesNotifications { get; set; }
        public bool ScheduleNotifications { get; set; }
        public bool CampusNotifications { get; set; }
        public bool NewsNotifications { get; set; }

        /// <summary>
        /// Computed property die aangeeft of minstens één notificatie aan staat.
        /// Kan later gebruikt worden voor push notification token logica.
        /// </summary>
        public bool IsEnabled => GradesNotifications || ScheduleNotifications || CampusNotifications || NewsNotifications;
    }
}

