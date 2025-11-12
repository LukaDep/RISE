using System.Net.Http.Json;
using Rise.Shared.Common;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications;

public class NotificationPreferencesClientService(HttpClient httpClient) : INotificationPreferencesService
{
    private readonly HttpClient httpClient = httpClient;

    public async Task<Result> EditAsync(NotificationPreferencesRequest.Edit req, CancellationToken ctx)
    {
        var response = await httpClient.PutAsJsonAsync("/api/notifications/preferences", req, cancellationToken: ctx);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: ctx);
            return result ?? Result.Success();
        }

        return Result.Error($"Fout bij het opslaan van notificatie-instellingen: {response.StatusCode}");
    }

    public async Task<Result<NotificationPreferencesResponse.Index>> GetByUserIdAsync(CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<NotificationPreferencesResponse.Index>>($"/api/notifications/preferences", cancellationToken: ctx);

        return result!;
    }
}
