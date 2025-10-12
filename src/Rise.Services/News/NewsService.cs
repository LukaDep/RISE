using System.Text.Json;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Services.News;

/// <summary>
/// Service for products.
/// </summary>
/// <param name="dbContext"></param>
public class NewsService(ApplicationDbContext dbContext) : INewsService
{
    private readonly string _mockFilePath = Path.Combine(Directory.GetCurrentDirectory(),"..", "Rise.Services", "News", "MockData", "news.json");
    public async Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        if (!File.Exists(_mockFilePath))
            return Result<NewsResponse.Index>.NotFound($"Mock data file not found at: {_mockFilePath}");

        var json = await File.ReadAllTextAsync(_mockFilePath, ctx);
        var items = JsonSerializer.Deserialize<List<NewsDto.Index>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        var response = new NewsResponse.Index { News = items };
        return Result.Success(response);
    }
}