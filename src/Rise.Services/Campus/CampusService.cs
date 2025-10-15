using System.Globalization;
using System.Text.Json;
using Rise.Persistence;
using Rise.Shared.Campus;
using Rise.Shared.Common;
using System.Text.Json.Serialization;

namespace Rise.Services.Campus;

// <summary>
// Service for campus.
// </summary>
// <param name="dbContext"></param>
public class CampusService() : ICampusService
{

    private readonly string _mockFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Rise.Services", "Campus", "MockData", "campus.json");
    public async Task<Result<CampusResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        if (!File.Exists(_mockFilePath))
            return Result<CampusResponse.Index>.NotFound($"Mock data file not found at: {_mockFilePath}");

        var json = await File.ReadAllTextAsync(_mockFilePath, ctx);
        var items = JsonSerializer.Deserialize<List<CampusDto.Index>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        var response = new CampusResponse.Index { Campuses = items };
        return Result.Success(response);
    }

    public async Task<Result<CampusDto.Index>> GetCampusByIdAsync(string id, CancellationToken ct = default)
    {
        if (!File.Exists(_mockFilePath))
            return Result<CampusDto.Index>.NotFound($"Mock data file not found at: {_mockFilePath}");

        var json = await File.ReadAllTextAsync(_mockFilePath, ct);
        var items = JsonSerializer.Deserialize<List<CampusDto.Index>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        var campus = items.FirstOrDefault(c => string.Equals(c.Id?.ToString(), id, StringComparison.OrdinalIgnoreCase));
        if (campus == null)
            return Result<CampusDto.Index>.NotFound($"Campus not found. Id: {id}");

        return Result.Success(campus);
    }


}