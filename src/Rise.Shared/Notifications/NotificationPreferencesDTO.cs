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
        public bool IsEnabled { get; set; }
    }
}

