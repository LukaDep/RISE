namespace Rise.Shared.Notifications;

/// <summary>
/// Response wrappers for notification preferences operations.
/// </summary>
public static class NotificationPreferencesResponse
{
    /// <summary>
    /// Response containing a user's notification preferences.
    /// Used for retrieving current preference settings.
    /// </summary>
    public class Index
    {
        public required NotificationPreferencesDTO.Index NotificationPreference { get; set; }
    }
}
