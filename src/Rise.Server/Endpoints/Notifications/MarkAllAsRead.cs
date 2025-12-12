using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om alle notificaties als gelezen te markeren.
/// </summary>
public class MarkAllAsRead(ISentNotificationService sentNotificationService)
    : EndpointWithoutRequest<Result>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Post("/api/notifications/mark-all-read");
        AllowAnonymous();
    }

    /// <summary>
    /// Marks all notifications as read for the current user.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    public override async Task<Result> ExecuteAsync(CancellationToken ct)
    {
        return await sentNotificationService.MarkAllAsReadAsync(ct);
    }
}
