using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om de notification preferences op te halen voor de ingelogde gebruiker.
/// </summary>
public class Index(INotificationPreferencesService notificationPreferencesService)
    : EndpointWithoutRequest<Result<NotificationPreferencesResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/notifications/preferences");
        // Ik heb hier allow anonymous gedaan om dat er toch op backend side gecheckt word of iemand is ingelogd of niet
        AllowAnonymous();
    }

    public override async Task<Result<NotificationPreferencesResponse.Index>> ExecuteAsync(CancellationToken ct)
    {
        return await notificationPreferencesService.GetUserPreferencesByIdAsync(ct);
    }
}
