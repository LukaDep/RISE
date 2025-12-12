using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Endpoint om notificaties in te kunnen schakelen.
/// </summary>
public class SendTest(INotificationPreferencesService notificationPreferencesService)
    : Endpoint<Push.Send, Result>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Post("/api/push/test");
        AllowAnonymous();
    }

    /// <summary>
    /// Sends a test push notification to the user.
    /// </summary>
    /// <param name="req">The request containing the test notification details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    public override async Task<Result> ExecuteAsync(Push.Send req, CancellationToken ct)
    {
        return await notificationPreferencesService.SendTestToUser(req, ct);
    }
}
