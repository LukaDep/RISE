using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om het aantal ongelezen notificaties op te halen.
/// </summary>
public class GetUnreadCount(ISentNotificationService sentNotificationService)
    : EndpointWithoutRequest<Result<int>>
{
    public override void Configure()
    {
        Get("/api/notifications/unread-count");
        AllowAnonymous();
    }

    public override async Task<Result<int>> ExecuteAsync(CancellationToken ct)
    {
        return await sentNotificationService.GetUnreadCountAsync(ct);
    }
}
