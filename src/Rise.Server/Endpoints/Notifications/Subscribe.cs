using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om notificaties in te kunnen schakelen.
/// </summary>
public class Subscribe(INotificationPreferencesService notificationPreferencesService)
    : Endpoint<PushSubscriptionRequest.Create, Result>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Post("/api/push/subscribe");
        AllowAnonymous();
    }

    /// <summary>
    /// Subscribes the user to push notifications.
    /// </summary>
    /// <param name="req">The subscription request containing push subscription details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    public override async Task<Result> ExecuteAsync(PushSubscriptionRequest.Create req, CancellationToken ct)
    {
        return await notificationPreferencesService.Subscribe(req, ct);
    }
}
