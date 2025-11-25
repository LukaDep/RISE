using System.Net.Http.Json;
using Microsoft.JSInterop;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications;

public class NotificationPreferencesClientService(HttpClient httpClient, IJSRuntime jsRuntime) : INotificationPreferencesService
{
    private readonly HttpClient httpClient = httpClient;
    private readonly IJSRuntime JS = jsRuntime;

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

    public async Task<Result<NotificationPreferencesResponse.Index>> GetUserPreferencesByIdAsync(CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<NotificationPreferencesResponse.Index>>($"/api/notifications/preferences", cancellationToken: ctx);

        return result!;
    }

    public Task<Result> SendTestToUser(Push.Send req, CancellationToken ctx = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> Subscribe(PushSubscriptionRequest.Create req, CancellationToken ctx = default)
    {
        try
        {
            var pubKey = "BCW-qlnpFfIjUDSN5cg0JUah1ktLevpGuU0HgBLvj76rpPinTndjtmEjZriWPsooBzKIJ4oEsTs8c1yAyCHBwGI";
            var subscribeObject = await JS.InvokeAsync<PushSubscriptionRequest.Create>("subscribeUser", pubKey);

            var response = await httpClient.PostAsJsonAsync("/api/push/subscribe", subscribeObject, ctx);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: ctx);
                return result ?? Result.Success();
            }

            return Result.Error($"Fout bij het aanmelden voor push-meldingen: {response.StatusCode}");
        }
        catch (JSException jsEx)
        {
            var errorMsg = string.IsNullOrEmpty(jsEx.Message) ? "Onbekende JavaScript fout" : jsEx.Message;
            Console.WriteLine($"JavaScript error during push subscription: {errorMsg}");
            return Result.Error($"Push-meldingen worden niet ondersteund of zijn geblokkeerd: {errorMsg}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error during push subscription: {ex.Message}");
            return Result.Error($"Onverwachte fout bij aanmelden voor push-meldingen: {ex.Message}");
        }
    }

    public async Task<Result> Unsubscribe(CancellationToken ctx = default)
    {
        try
        {
            var response = await httpClient.DeleteAsync("/api/push/unsubscribe", ctx);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: ctx);
                return result ?? Result.Success();
            }

            return Result.Error($"Fout bij het afmelden voor push-meldingen: {response.StatusCode}");
        }
        catch (JSException jsEx)
        {
            var errorMsg = string.IsNullOrEmpty(jsEx.Message) ? "Onbekende JavaScript fout" : jsEx.Message;
            Console.WriteLine($"JavaScript error during push unsubscription: {errorMsg}");
            return Result.Error($"Push-meldingen worden niet ondersteund of zijn geblokkeerd: {errorMsg}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error during push unsubscription: {ex.Message}");
            return Result.Error($"Onverwachte fout bij afmelden voor push-meldingen: {ex.Message}");
        }
    }
}
