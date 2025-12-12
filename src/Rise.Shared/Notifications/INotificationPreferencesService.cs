namespace Rise.Shared.Notifications;

/// <summary>
/// Service interface for managing notification preferences and push subscriptions.
/// </summary>
public interface INotificationPreferencesService
{
    /// <summary>
    /// Retrieves the notification preferences for the current user.
    /// Creates default preferences if they don't exist yet.
    /// </summary>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with NotificationPreferencesResponse.Index containing preferences and subscription status, or Unauthorized if not logged in</returns>
    Task<Result<NotificationPreferencesResponse.Index>> GetUserPreferencesByIdAsync(CancellationToken ctx = default);

    /// <summary>
    /// Updates the notification preferences for the current user.
    /// </summary>
    /// <param name="req">NotificationPreferencesRequest.Edit with the new preference settings</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result.Success on successful update, Unauthorized if not logged in, NotFound if preferences don't exist</returns>
    Task<Result> EditAsync(NotificationPreferencesRequest.Edit req, CancellationToken ctx);

    /// <summary>
    /// Registers or updates a push subscription for the current user.
    /// </summary>
    /// <param name="req">PushSubscriptionRequest.Create with endpoint and keys</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result.Success on successful registration, Unauthorized if not logged in</returns>
    Task<Result> Subscribe(PushSubscriptionRequest.Create req, CancellationToken ctx = default);

    /// <summary>
    /// Removes all push subscriptions for the current user.
    /// </summary>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result.Success on successful removal, Unauthorized if not logged in</returns>
    Task<Result> Unsubscribe(CancellationToken ctx = default);

    /// <summary>
    /// Sends a test push notification to a specific user or to all users.
    /// Saves the sent notification with delivery status.
    /// </summary>
    /// <param name="req">Push.Send with userGuid (null for all users), title, body, url and notificationType</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result.Success on successful send</returns>
    Task<Result> SendTestToUser(Push.Send req, CancellationToken ctx = default);

    /// <summary>
    /// Synchronizes the existing push subscription with the server after login.
    /// This re-registers the browser's existing push subscription without asking for permission again.
    /// </summary>
    Task<Result> SyncSubscriptionAsync(CancellationToken ctx = default);
}