using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om het aantal ongelezen notificaties op te halen.
/// </summary>
public class GetUnreadCount(ISentNotificationService sentNotificationService)
    : EndpointWithoutRequest<Result<int>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/notifications/unread-count");
        AllowAnonymous();
    }

    /// <summary>
    /// Retrieves the count of unread notifications for the current user.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the unread notification count.</returns>
    public override async Task<Result<int>> ExecuteAsync(CancellationToken ct)
    {
        return await sentNotificationService.GetUnreadCountAsync(ct);
    }
}
