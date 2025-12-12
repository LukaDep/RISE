using System.Net.Http.Json;
using Rise.Shared.Menu;
using Rise.Shared.Common;
namespace Rise.Client.Menu;

/// <summary>
/// Client-side service for menu operations.
/// </summary>
/// <param name="httpClient">The HTTP client for API communication.</param>
public class MenuClientService(HttpClient httpClient) : IMenuService
{
    /// <summary>
    /// Retrieves the menu items.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result containing the menu data.</returns>
    public async Task<Result<MenuResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<MenuResponse.Index>>("/api/menus", cancellationToken: ctx);
        return result!;
    }
}
