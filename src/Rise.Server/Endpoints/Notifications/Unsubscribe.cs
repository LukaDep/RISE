using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om notificaties uit te kunnen schakelen.
/// </summary>
public class Unsubscribe(INotificationPreferencesService notificationPreferencesService)
    : EndpointWithoutRequest<Result>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/push/unsubscribe");
        AllowAnonymous();
    }

    /// <summary>
    /// Unsubscribes the user from push notifications.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    public override async Task<Result> ExecuteAsync(CancellationToken ct)
    {
        return await notificationPreferencesService.Unsubscribe(ct);
    }
}