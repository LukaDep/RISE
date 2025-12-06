using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Rise.Shared.Notifications;

namespace Rise.Client.Account.Notifications;

public partial class Index : ComponentBase
{
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

    protected override async Task OnInitializedAsync()
    {
        await LoadNotificationsAsync();
    }

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

    private string GetNotificationClass(SentNotificationDTO.Index notification)
    {
        return notification.IsRead
            ? "hover:bg-gray-50"
            : "bg-blue-50/50 hover:bg-blue-50";
    }

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

    private string GetTitleClass(SentNotificationDTO.Index notification)
    {
        return notification.IsRead
            ? "font-medium text-gray-700"
            : "font-semibold text-gray-900";
    }

    private string FormatDate(DateTime sentAt)
    {
        var now = DateTime.UtcNow;
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
