using Rise.Shared.Identity;
using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Edit a <see cref="NotificationPreferences"/>.
/// </summary>
/// <param name="NotificationPreferencesService"></param>
public class Edit(INotificationPreferencesService NotificationPreferencesService) : Endpoint<NotificationPreferencesRequest.Edit, Result>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Put("/api/notifications/preferences");
        // Ik heb hier allow anonymous gedaan om dat er toch op backend side gecheckt word of iemand is ingelogd of niet
        Roles(AppRoles.Student);
    }

    /// <summary>
    /// Updates the notification preferences for the current user.
    /// </summary>
    /// <param name="req">The edit request containing the updated preferences.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    public override Task<Result> ExecuteAsync(NotificationPreferencesRequest.Edit req, CancellationToken ctx)
    {
        return NotificationPreferencesService.EditAsync(req, ctx);
    }
}