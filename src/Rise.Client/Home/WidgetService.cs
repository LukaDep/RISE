using System.Net.Http.Json;
using Rise.Shared.Widgets;

namespace Rise.Client.Home;

public class WidgetService(HttpClient httpClient) : IWidgetService
{
    public async Task<Result<WidgetResponse.Index>> GetIndexByUserIdAsync(CancellationToken ctx = default)
    {
        var response = await httpClient.GetAsync($"/api/widgets", cancellationToken: ctx);
        var result = await response.Content.ReadFromJsonAsync<Result<WidgetResponse.Index>>(cancellationToken: ctx);
        return result!;
    }

    public async Task<Result> UpdateUserWidgetsAsync(WidgetRequest.Update request, CancellationToken ctx = default)
    {
        var response = await httpClient.PutAsJsonAsync("/api/widgets", request, ctx);
        var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: ctx);
        return result!;
    }
}