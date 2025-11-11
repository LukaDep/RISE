using System.Net.Http.Json;
using Rise.Shared.Menu;
using Rise.Shared.Common;
namespace Rise.Client.Menu;

public class MenuClientService(HttpClient httpClient) : IMenuService
{
    public async Task<Result<MenuResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<MenuResponse.Index>>("/api/menus", cancellationToken: ctx);
        return result!;
    }
}
