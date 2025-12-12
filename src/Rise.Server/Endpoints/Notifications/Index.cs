using Rise.Shared.Identity;
using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om de notification preferences op te halen voor de ingelogde gebruiker.
/// </summary>
public class Index(INotificationPreferencesService notificationPreferencesService)
    : EndpointWithoutRequest<Result<NotificationPreferencesResponse.Index>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/notifications/preferences");
        // Ik heb hier allow anonymous gedaan om dat er toch op backend side gecheckt word of iemand is ingelogd of niet
        Roles(AppRoles.Student);
    }

    /// <summary>
    /// Retrieves the notification preferences for the current user.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the user's notification preferences.</returns>
    public override async Task<Result<NotificationPreferencesResponse.Index>> ExecuteAsync(CancellationToken ct)
    {
        return await notificationPreferencesService.GetUserPreferencesByIdAsync(ct);
    }
}
