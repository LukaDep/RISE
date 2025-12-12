using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om een notificatie als gelezen te markeren.
/// </summary>
public class MarkAsRead(ISentNotificationService sentNotificationService)
    : Endpoint<SentNotificationRequest.MarkAsRead, Result>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Post("/api/notifications/mark-read");
        AllowAnonymous();
    }

    /// <summary>
    /// Marks a specific notification as read.
    /// </summary>
    /// <param name="req">The request containing the notification ID to mark as read.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    public override async Task<Result> ExecuteAsync(SentNotificationRequest.MarkAsRead req, CancellationToken ct)
    {
        return await sentNotificationService.MarkAsReadAsync(req, ct);
    }
}
