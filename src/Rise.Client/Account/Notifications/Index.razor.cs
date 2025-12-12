using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Rise.Shared.Notifications;

namespace Rise.Client.Account.Notifications;

/// <summary>
/// Code-behind for the Notifications Index page component.
/// Displays a paginated list of user notifications with read/unread status.
/// </summary>
public partial class Index : ComponentBase
{
    /// <summary>Service for sent notification operations.</summary>
    [Inject]
    public required ISentNotificationService SentNotificationService { get; set; }

    // L is injected via _Imports.razor - use it directly in the partial class

    private List<SentNotificationDTO.Index>? notifications;
    private bool isLoading = true;
    private bool isLoadingMore = false;
    private string? errorMessage;
    private int currentPage = 1;
    private int pageSize = 20;
    private int totalCount = 0;
    private int unreadCount = 0;
    private bool hasMoreNotifications = false;

    /// <summary>
    /// Loads notifications on component initialization.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadNotificationsAsync();
    }

    /// <summary>
    /// Loads paginated notifications from the service.
    /// </summary>
    private async Task LoadNotificationsAsync()
    {
        try
        {
            isLoading = true;
            errorMessage = null;

            var result = await SentNotificationService.GetUserNotificationsAsync(currentPage, pageSize);

            if (result.IsSuccess && result.Value != null)
            {
                notifications = result.Value.Notifications.ToList();
                totalCount = result.Value.TotalCount;
                unreadCount = result.Value.UnreadCount;
                hasMoreNotifications = notifications.Count < totalCount;
            }
            else
            {
                errorMessage = result.Errors.FirstOrDefault() ?? "Kon notificaties niet laden.";
                notifications = null;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout bij het laden van notificaties");
            errorMessage = "Er is een onverwachte fout opgetreden bij het laden van notificaties.";
        }
        finally
        {
            isLoading = false;
        }
    }

    /// <summary>
    /// Loads the next page of notifications for infinite scroll pagination.
    /// Increments the current page counter and appends new notifications to the existing list.
    /// Automatically tracks whether more notifications are available based on total count.
    /// </summary>
    private async Task LoadMore()
    {
        if (isLoadingMore || !hasMoreNotifications)
            return;

        try
        {
            isLoadingMore = true;
            currentPage++;

            var result = await SentNotificationService.GetUserNotificationsAsync(currentPage, pageSize);

            if (result.IsSuccess && result.Value != null)
            {
                notifications?.AddRange(result.Value.Notifications);
                hasMoreNotifications = (notifications?.Count ?? 0) < totalCount;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout bij het laden van meer notificaties");
            currentPage--;
        }
        finally
        {
            isLoadingMore = false;
        }
    }

    /// <summary>
    /// Handles notification click events. Marks the notification as read if unread,
    /// updates the local unread count, and navigates to the notification's target URL if present.
    /// The read status is updated optimistically for better UX, with the actual timestamp
    /// being set server-side.
    /// </summary>
    /// <param name="notification">The notification that was clicked.</param>
    private async Task OnNotificationClick(SentNotificationDTO.Index notification)
    {
        // Mark as read if not already
        if (!notification.IsRead)
        {
            var result = await SentNotificationService.MarkAsReadAsync(new SentNotificationRequest.MarkAsRead
            {
                NotificationId = notification.Id
            });

            if (result.IsSuccess)
            {
                notification.IsRead = true;
                // Note: ReadAt is set by the server - we only update IsRead for UI purposes.
                // The actual timestamp will be fetched on next data refresh.
                unreadCount = Math.Max(0, unreadCount - 1);
            }
        }

        // Navigate to URL if present
        if (!string.IsNullOrEmpty(notification.Url))
        {
            NavigationManager.NavigateTo(notification.Url);
        }
    }

    /// <summary>
    /// Marks all notifications as read in a single batch operation.
    /// Updates the local state optimistically by setting IsRead=true on all notifications
    /// and resetting the unread count to zero. Server handles the actual timestamp updates.
    /// </summary>
    private async Task MarkAllAsRead()
    {
        try
        {
            var result = await SentNotificationService.MarkAllAsReadAsync();

            if (result.IsSuccess)
            {
                if (notifications != null)
                {
                    foreach (var n in notifications)
                    {
                        n.IsRead = true;
                        // Note: ReadAt is set by the server - we only update IsRead for UI purposes.
                        // The actual timestamps will be fetched on next data refresh.
                    }
                }
                unreadCount = 0;
            }
            else
            {
                errorMessage = result.Errors.FirstOrDefault() ?? "Kon notificaties niet markeren als gelezen.";
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout bij het markeren van alle notificaties als gelezen");
            errorMessage = "Er is een fout opgetreden.";
        }
    }

    /// <summary>
    /// Deletes a single notification by its ID.
    /// Removes the notification from the local list, updates the total count,
    /// and decrements the unread count if the deleted notification was unread.
    /// </summary>
    /// <param name="notificationId">The unique identifier of the notification to delete.</param>
    private async Task DeleteNotification(Guid notificationId)
    {
        try
        {
            var result = await SentNotificationService.DeleteNotificationAsync(notificationId);

            if (result.IsSuccess)
            {
                var notification = notifications?.FirstOrDefault(n => n.Id == notificationId);
                if (notification != null)
                {
                    if (!notification.IsRead)
                    {
                        unreadCount = Math.Max(0, unreadCount - 1);
                    }
                    notifications?.Remove(notification);
                    totalCount = Math.Max(0, totalCount - 1);
                }
            }
            else
            {
                errorMessage = result.Errors.FirstOrDefault() ?? "Kon notificatie niet verwijderen.";
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout bij het verwijderen van notificatie");
            errorMessage = "Er is een fout opgetreden.";
        }
    }

    /// <summary>
    /// Gets the CSS class for a notification item based on its read status.
    /// Unread notifications have a blue tinted background for visual distinction.
    /// </summary>
    /// <param name="notification">The notification to get styling for.</param>
    /// <returns>CSS classes for the notification container.</returns>
    private string GetNotificationClass(SentNotificationDTO.Index notification)
    {
        return notification.IsRead
            ? "hover:bg-gray-50"
            : "bg-blue-50/50 hover:bg-blue-50";
    }

    /// <summary>
    /// Gets the CSS classes for the notification icon container.
    /// Applies a colored gradient background based on notification type:
    /// grades=emerald, schedule=blue, campus=purple, news=orange, other=gray.
    /// </summary>
    /// <param name="notification">The notification to style.</param>
    /// <returns>CSS classes for the icon container including type-specific gradient.</returns>
    private string GetIconContainerClass(SentNotificationDTO.Index notification)
    {
        var baseClass = "w-8 h-8 rounded-full flex items-center justify-center shadow-sm";
        var gradientClass = notification.NotificationType switch
        {
            "grades" => "bg-gradient-to-br from-emerald-400 to-emerald-500",
            "schedule" => "bg-gradient-to-br from-blue-400 to-blue-500",
            "campus" => "bg-gradient-to-br from-purple-400 to-purple-500",
            "news" => "bg-gradient-to-br from-orange-400 to-orange-500",
            _ => "bg-gradient-to-br from-gray-400 to-gray-500"
        };

        return $"{baseClass} {gradientClass}";
    }

    /// <summary>
    /// Gets the Font Awesome icon class for a notification based on its type.
    /// Maps notification types to appropriate icons: grades=graduation-cap,
    /// schedule=calendar, campus=school, news=newspaper, other=bell.
    /// </summary>
    /// <param name="notification">The notification to get an icon for.</param>
    /// <returns>Font Awesome icon class string.</returns>
    private string GetNotificationIcon(SentNotificationDTO.Index notification)
    {
        return notification.NotificationType switch
        {
            "grades" => "fa-solid fa-graduation-cap",
            "schedule" => "fa-solid fa-calendar-days",
            "campus" => "fa-solid fa-school",
            "news" => "fa-solid fa-newspaper",
            _ => "fa-solid fa-bell"
        };
    }

    /// <summary>
    /// Gets the CSS class for the notification title based on read status.
    /// Unread notifications have bolder, darker text for emphasis.
    /// </summary>
    /// <param name="notification">The notification to style.</param>
    /// <returns>CSS classes for title text styling.</returns>
    private string GetTitleClass(SentNotificationDTO.Index notification)
    {
        return notification.IsRead
            ? "font-medium text-gray-700"
            : "font-semibold text-gray-900";
    }

    /// <summary>
    /// Formats a notification timestamp into a human-readable relative time string.
    /// Shows "just now" for &lt;1min, "X minutes ago" for &lt;1hr, "X hours ago" for &lt;24hr,
    /// "X days ago" for &lt;7 days, and full date (dd MMM yyyy) for older notifications.
    /// Uses localized strings for display.
    /// </summary>
    /// <param name="sentAt">The timestamp when the notification was sent.</param>
    /// <returns>Localized relative time string.</returns>
    private string FormatDate(DateTime sentAt)
    {
        var now = DateTime.Now;
        var diff = now - sentAt;

        if (diff.TotalMinutes < 1)
            return L["Time.JustNow"];
        if (diff.TotalMinutes < 60)
            return L["Time.MinutesAgo", (int)diff.TotalMinutes];
        if (diff.TotalHours < 24)
            return L["Time.HoursAgo", (int)diff.TotalHours];
        if (diff.TotalDays < 7)
            return L["Time.DaysAgo", (int)diff.TotalDays];

        return sentAt.ToString("dd MMM yyyy");
    }
}
