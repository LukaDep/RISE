namespace Rise.Shared.Notifications;

/// <summary>
/// Provides methods for managing NotificationPreferences-related operations.
/// </summary>
public interface INotificationPreferencesService
{
    Task<Result<NotificationPreferencesResponse.Index>> GetUserPreferencesByIdAsync(CancellationToken ctx = default);
    Task<Result> EditAsync(NotificationPreferencesRequest.Edit req, CancellationToken ctx);
    Task<Result> Subscribe(PushSubscriptionRequest.Create req, CancellationToken ctx = default);
    Task<Result> Unsubscribe(CancellationToken ctx = default);
    Task<Result> SendTestToUser(Push.Send req, CancellationToken ctx = default);

    /// <summary>
    /// Synchronizes the existing push subscription with the server after login.
    /// This re-registers the browser's existing push subscription without asking for permission again.
    /// </summary>
    Task<Result> SyncSubscriptionAsync(CancellationToken ctx = default);
}