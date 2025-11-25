using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om notificaties in te kunnen schakelen.
/// </summary>
public class SendTest(INotificationPreferencesService notificationPreferencesService)
    : Endpoint<Push.Send, Result>
{
    public override void Configure()
    {
        Post("/api/push/test");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(Push.Send req, CancellationToken ct)
    {
        return await notificationPreferencesService.SendTestToUser(req, ct);
    }
}
