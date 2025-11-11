using System.Text.Json;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.CampusInfo;

namespace Rise.Services.CampusInfo;

/// <summary>
/// Service for campusInfo.
/// </summary>
/// <param name="dbContext"></param>
public class CampusInfoService(ApplicationDbContext dbContext) : ICampusInfoService
{
    private readonly string _mockFilePath = Path.Combine(Directory.GetCurrentDirectory(), "CampusInfo", "MockData", "campusInfo.json");
    public async Task<Result<CampusInfoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        if (!File.Exists(_mockFilePath))
            return Result<CampusInfoResponse.Index>.NotFound($"Mock data file not found at: {_mockFilePath}");

        var json = await File.ReadAllTextAsync(_mockFilePath, ctx);
        var items = JsonSerializer.Deserialize<List<CampusInfoDto.Index>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        var response = new CampusInfoResponse.Index { CampusInfo = items };
        return Result.Success(response);
    }
}
