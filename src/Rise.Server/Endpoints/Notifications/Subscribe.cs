using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om notificaties in te kunnen schakelen.
/// </summary>
public class Subscribe(INotificationPreferencesService notificationPreferencesService)
    : Endpoint<PushSubscriptionRequest.Create, Result>
{
    public override void Configure()
    {
        Post("/api/push/subscribe");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(PushSubscriptionRequest.Create req, CancellationToken ct)
    {
        return await notificationPreferencesService.Subscribe(req, ct);
    }
}
