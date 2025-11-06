using Rise.Shared.Campus;
using System.Net.Http.Json;
using Rise.Shared.Common;

namespace Rise.Client.Campus;

public class CampusClientService(HttpClient httpClient) : ICampusService
{
    public async Task<Result<CampusResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<CampusResponse.Index>>($"/api/campuses", cancellationToken: ctx);
        Console.WriteLine("Fetching campus data from API...");
        return result!;
    }

    public async Task<Result<CampusDto.Index>> GetCampusByIdAsync(Guid buildingId, CancellationToken ct = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<BuildingDto.Index>>($"/api/buildings/{buildingId}", cancellationToken: ct);
        Console.WriteLine($"Fetching building data for ID {buildingId} from API...");
        return result!;
    }
}