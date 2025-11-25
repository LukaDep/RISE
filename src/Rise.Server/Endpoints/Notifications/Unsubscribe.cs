using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om notificaties uit te kunnen schakelen.
/// </summary>
public class Unsubscribe(INotificationPreferencesService notificationPreferencesService)
    : EndpointWithoutRequest<Result>
{
    public override void Configure()
    {
        Delete("/api/push/unsubscribe");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(CancellationToken ct)
    {
        return await notificationPreferencesService.Unsubscribe(ct);
    }
}