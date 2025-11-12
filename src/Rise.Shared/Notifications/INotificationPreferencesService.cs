using Rise.Shared.Common;

namespace Rise.Shared.Notifications;

/// <summary>
/// Provides methods for managing NotificationPreferences-related operations.
/// </summary>
public interface INotificationPreferencesService
{
    Task<Result<NotificationPreferencesResponse.Index>> GetByUserIdAsync(CancellationToken ctx = default);
    Task<Result> EditAsync(NotificationPreferencesRequest.Edit req, CancellationToken ctx);
}