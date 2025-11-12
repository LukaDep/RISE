using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Edit a <see cref="NotificationPreferences"/>.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="NotificationPreferencesService"></param>
public class Edit(INotificationPreferencesService NotificationPreferencesService) : Endpoint<NotificationPreferencesRequest.Edit, Result>
{
    public override void Configure()
    {
        Put("/api/notifications/preferences");
        // Ik heb hier allow anonymous gedaan om dat er toch op backend side gecheckt word of iemand is ingelogd of niet
        AllowAnonymous();
    }

    public override Task<Result> ExecuteAsync(NotificationPreferencesRequest.Edit req, CancellationToken ctx)
    {
        return NotificationPreferencesService.EditAsync(req, ctx);
    }
}