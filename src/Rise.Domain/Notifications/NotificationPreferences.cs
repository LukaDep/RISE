namespace Rise.Domain.Notifications;

public class NotificationPreferences : Entity
{
    public NotificationPreferences(Guid id)
    {
        Id = id;
    }

    public bool GradesNotifications { get; set; } = true;
    public bool ScheduleNotifications { get; set; } = true;
    public bool CampusNotifications { get; set; } = true;
    public bool NewsNotifications { get; set; } = true;

}
