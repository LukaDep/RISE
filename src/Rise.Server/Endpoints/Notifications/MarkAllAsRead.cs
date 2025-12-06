using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om alle notificaties als gelezen te markeren.
/// </summary>
public class MarkAllAsRead(ISentNotificationService sentNotificationService)
    : EndpointWithoutRequest<Result>
{
    public override void Configure()
    {
        Post("/api/notifications/mark-all-read");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(CancellationToken ct)
    {
        return await sentNotificationService.MarkAllAsReadAsync(ct);
    }
}
