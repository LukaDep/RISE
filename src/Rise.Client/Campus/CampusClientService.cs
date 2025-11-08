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
        Console.WriteLine(result);
        return result?.Value!;
    }

    public async Task<Result<CampusResponse.Get>> GetCampusByIdAsync(Guid buildingId, CancellationToken ct = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<CampusResponse.Get>>($"/api/campuses/{buildingId}", cancellationToken: ct);
        return result!;
    }

    public async Task<Result<BuildingResponse.Get>> GetBuildingByIdAsync(Guid id, CancellationToken ct = default)
    {
        
        var result = await httpClient.GetFromJsonAsync<Result<BuildingResponse.Get>>($"/api/buildings/{id}", cancellationToken: ct);
        Console.WriteLine($"Fetching building data for ID {id} from API...");
        return result!;
    }
}