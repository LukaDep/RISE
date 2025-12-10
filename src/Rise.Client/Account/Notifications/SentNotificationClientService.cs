using System.Net.Http.Json;
using Rise.Shared.Notifications;

namespace Rise.Client.Account.Notifications;

public class SentNotificationClientService(HttpClient httpClient) : ISentNotificationService
{
    public async Task<Result<SentNotificationResponse.Index>> GetUserNotificationsAsync(int page = 1, int pageSize = 20, CancellationToken ctx = default)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<Result<SentNotificationResponse.Index>>(
                $"/api/notifications/sent?page={page}&pageSize={pageSize}",
                cancellationToken: ctx);

            return result ?? Result.Error("Kon notificaties niet ophalen.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching notifications: {ex.Message}");
            return Result.Error($"Fout bij het ophalen van notificaties: {ex.Message}");
        }
    }

    public async Task<Result> MarkAsReadAsync(SentNotificationRequest.MarkAsRead req, CancellationToken ctx = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("/api/notifications/mark-read", req, cancellationToken: ctx);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: ctx);
                return result ?? Result.Success();
            }

            return Result.Error($"Fout bij het markeren als gelezen: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error marking notification as read: {ex.Message}");
            return Result.Error($"Fout bij het markeren als gelezen: {ex.Message}");
        }
    }

    public async Task<Result> MarkAllAsReadAsync(CancellationToken ctx = default)
    {
        try
        {
            var response = await httpClient.PostAsync("/api/notifications/mark-all-read", null, cancellationToken: ctx);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: ctx);
                return result ?? Result.Success();
            }

            return Result.Error($"Fout bij het markeren van alle notificaties als gelezen: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error marking all notifications as read: {ex.Message}");
            return Result.Error($"Fout bij het markeren van alle notificaties als gelezen: {ex.Message}");
        }
    }

    public async Task<Result<int>> GetUnreadCountAsync(CancellationToken ctx = default)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<Result<int>>(
                "/api/notifications/unread-count",
                cancellationToken: ctx);

            return result ?? Result.Error("Kon ongelezen aantal niet ophalen.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching unread count: {ex.Message}");
            return Result.Error($"Fout bij het ophalen van ongelezen aantal: {ex.Message}");
        }
    }

    public async Task<Result> DeleteNotificationAsync(Guid notificationId, CancellationToken ctx = default)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"/api/notifications/{notificationId}", cancellationToken: ctx);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: ctx);
                return result ?? Result.Success();
            }

            return Result.Error($"Fout bij het verwijderen van notificatie: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting notification: {ex.Message}");
            return Result.Error($"Fout bij het verwijderen van notificatie: {ex.Message}");
        }
    }

    public Task SaveSentNotificationAsync(Guid pushSubscriptionId, string title, string body, string? url = null, string? notificationType = null, DeliveryStatus deliveryStatus = DeliveryStatus.Pending, CancellationToken ctx = default)
    {
        // This is only used server-side, client doesn't need to implement this
        throw new NotImplementedException("SaveSentNotificationAsync is only used server-side.");
    }
}
