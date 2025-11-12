namespace Rise.Shared.Notifications;

public static class NotificationPreferencesResponse
{
    public class Index
    {
        public required NotificationPreferencesDTO.Index NotificationPreference { get; set; }
    }
}
