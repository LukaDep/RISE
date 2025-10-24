using System.Net.Http.Json;
using Rise.Shared.Menus;
using Rise.Shared.Common;

namespace Rise.Client.Menus;

public class MenuClientService(HttpClient httpClient) : IMenuService
{
    public async Task<Result<MenuResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ct = default)
    {
        var url = $"/api/menus?skip={request.Skip}&take={request.Take}";
        var result = await httpClient.GetFromJsonAsync<Result<MenuResponse.Index>>(url, cancellationToken: ct);

        Console.WriteLine($"📡 Fetching menu data (skip={request.Skip}, take={request.Take}) from API...");
        return result!;
    }
}
