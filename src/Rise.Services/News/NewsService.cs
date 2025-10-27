using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Services.News;

/// <summary>
/// Service for News.
/// </summary>
/// <param name="dbContext"></param>
public class NewsService(ApplicationDbContext dbContext) : INewsService
{
    private readonly string _mockFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Rise.Services", "News", "MockData", "news.json");
    public async Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        //read from mock json file
        if (!File.Exists(_mockFilePath))
            return Result<NewsResponse.Index>.NotFound($"Mock data file not found at: {_mockFilePath}");
        var json = await File.ReadAllTextAsync(_mockFilePath, ctx);

        // Deserialize the JSON data into a list of NewsDto.Index
        var query = JsonSerializer.Deserialize<List<NewsDto.Index>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(n => n.Title.Contains(request.SearchTerm, StringComparison.CurrentCultureIgnoreCase)
                                     || n.Author.Contains(request.SearchTerm, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }
        // Apply ordering
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy)).ToList()
                : query.OrderBy(e => EF.Property<object>(e, request.OrderBy)).ToList();
        }
        else
        {
            // Default order
            query = query.OrderBy(p => p.Title).ToList();
        }
        var totalCount = query.Count();

        var news = query
            .Skip(request.Skip)
            .Take(request.Take)
            .ToList();
        var currentCount = news.Count;
        

        return Result.Success(new NewsResponse.Index
        {
            News = news,
            TotalCount = totalCount,
            CurrentCount = currentCount,
        }
        );
    }

    public async Task<Result<NewsResponse.Get>> GetByIdAsync(int id, CancellationToken ctx = default)
    {
        if (!File.Exists(_mockFilePath))
            return Result<NewsResponse.Get>.NotFound($"Mock data file not found at: {_mockFilePath}");

        var json = await File.ReadAllTextAsync(_mockFilePath, ctx);
        var items = JsonSerializer.Deserialize<List<NewsDto.Index>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        var newsItem = items.FirstOrDefault(n => n.Id == id);
        if (newsItem == null)
            return Result<NewsResponse.Get>.NotFound($"News item with id {id} not found.");

        var response = new NewsResponse.Get { NewsItem = newsItem };
        return Result.Success(response);
    }
}