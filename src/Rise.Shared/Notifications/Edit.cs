namespace Rise.Shared.Notifications;

public static partial class NotificationPreferencesRequest
{
    public class Edit
    {
        /// <summary>
        /// Meldingen bij nieuwe of gewijzigde cijfers.
        /// </summary>
        public bool GradesNotifications { get; set; }

        /// <summary>
        /// Meldingen over rooster- of planningswijzigingen.
        /// </summary>
        public bool ScheduleNotifications { get; set; }

        /// <summary>
        /// Meldingen over campusgerelateerde aankondigingen.
        /// </summary>
        public bool CampusNotifications { get; set; }

        /// <summary>
        /// Meldingen over nieuws of algemene updates.
        /// </summary>
        public bool NewsNotifications { get; set; }
    }
}
