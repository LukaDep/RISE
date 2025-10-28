using Rise.Shared.Common;
using Rise.Shared.Resto;
using System.Net.Http.Json;

namespace Rise.Client.Resto;

public class RestoClientService(HttpClient httpClient) : IRestoService
{
    private readonly HttpClient httpClient = httpClient;

    public async Task<Result<RestoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<RestoResponse.Index>>($"/api/restos?skip={req.Skip}&take={req.Take}&searchTerm={req.SearchTerm ?? ""}", cancellationToken: ctx);
        return result!;
    }
}
