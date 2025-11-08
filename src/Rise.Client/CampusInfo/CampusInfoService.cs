using Rise.Shared.CampusInfo;
using System.Net.Http.Json;
using Rise.Shared.Common;

namespace Rise.Client.CampusInfo;

public class CampusInfoService(HttpClient httpClient) : ICampusInfoService
{
    public async Task<Result<CampusInfoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<CampusInfoResponse.Index>>($"/api/campus", cancellationToken: ctx);
        return result!;
    }
}