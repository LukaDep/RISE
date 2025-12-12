using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Request DTO for getting sent notifications with pagination.
/// </summary>
public class GetSentNotificationsRequest
{
    /// <summary>
    /// The page number (1-based). Defaults to 1.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// The number of items per page. Defaults to 20.
    /// </summary>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Endpoint om alle verzonden notificaties op te halen voor de ingelogde gebruiker.
/// </summary>
public class GetSentNotifications(ISentNotificationService sentNotificationService)
    : Endpoint<GetSentNotificationsRequest, Result<SentNotificationResponse.Index>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/notifications/sent");
        AllowAnonymous();
    }

    /// <summary>
    /// Retrieves a paginated list of sent notifications for the current user.
    /// </summary>
    /// <param name="req">The request containing pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the list of sent notifications.</returns>
    public override async Task<Result<SentNotificationResponse.Index>> ExecuteAsync(GetSentNotificationsRequest req, CancellationToken ct)
    {
        // Ensure pagination values are within reasonable bounds
        var page = Math.Max(1, req.Page);
        var pageSize = Math.Clamp(req.PageSize, 1, 100);

        return await sentNotificationService.GetUserNotificationsAsync(page, pageSize, ct);
    }
}
