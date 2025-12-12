using System.Net;
using System.Net.Http.Json;
using Rise.Shared.Widgets;

namespace Rise.Client.Home;

/// <summary>
/// Client-side service for widget operations.
/// </summary>
/// <param name="httpClient">The HTTP client for API communication.</param>
public class WidgetService(HttpClient httpClient) : IWidgetService
{
    /// <summary>
    /// Retrieves all widgets configured for the current user.
    /// </summary>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result containing the user's widget configuration, or an error if unauthorized.</returns>
    public async Task<Result<WidgetResponse.Index>> GetIndexByUserIdAsync(CancellationToken ctx = default)
    {

        var response = await httpClient.GetAsync($"/api/widgets", cancellationToken: ctx);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Result<WidgetResponse.Index>.Error("Not Authorized");
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Result<WidgetResponse.Index>>(cancellationToken: ctx);
        return result!;

    }

    /// <summary>
    /// Updates the widget configuration for the current user.
    /// </summary>
    /// <param name="request">The update request containing the new widget configuration.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    public async Task<Result> UpdateUserWidgetsAsync(WidgetRequest.Update request, CancellationToken ctx = default)
    {
        try
        {

            var response = await httpClient.PutAsJsonAsync("/api/widgets", request, ctx);
            var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: ctx);
            return result!;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return Result.Error("Not Authorized");

        }
    }
}