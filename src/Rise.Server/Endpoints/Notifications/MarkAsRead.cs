using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om een notificatie als gelezen te markeren.
/// </summary>
public class MarkAsRead(ISentNotificationService sentNotificationService)
    : Endpoint<SentNotificationRequest.MarkAsRead, Result>
{
    public override void Configure()
    {
        Post("/api/notifications/mark-read");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(SentNotificationRequest.MarkAsRead req, CancellationToken ct)
    {
        return await sentNotificationService.MarkAsReadAsync(req, ct);
    }
}
