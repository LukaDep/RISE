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
        // //read from mock json file
        // if (!File.Exists(_mockFilePath))
        //     return Result<NewsResponse.Index>.NotFound($"Mock data file not found at: {_mockFilePath}");
        // var json = await File.ReadAllTextAsync(_mockFilePath, ctx);
        //
        // // Deserialize the JSON data into a list of NewsDto.Index
        // var query = JsonSerializer.Deserialize<List<NewsDto.Index>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        var query = dbContext.NewsArticles.AsQueryable();



        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(n => n.Title.Contains(request.SearchTerm)
                                     || n.Author.Contains(request.SearchTerm));
        }
        // Apply ordering
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy))
                : query.OrderBy(e => EF.Property<object>(e, request.OrderBy));
        }
        else
        {
            // Default order
            query = query.OrderBy(p => p.Title);
        }
        var totalCount = await query.CountAsync(ctx);

        var news = await query.AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(n => new NewsDto.Index
            {
                Id = n.Id,
                Title = n.Title,
                Description = n.Description,
                Type = n.Type,
                PublishDate = n.PublishDate,
                Content = n.Content,
                Author = n.Author
            })
            .ToListAsync(ctx);



        return Result.Success(new NewsResponse.Index
        {
            News = news,
            TotalCount = totalCount,
        }
        );
    }

    public async Task<Result<NewsResponse.Get>> GetByIdAsync(Guid id, CancellationToken ctx = default)
    {
        // if (!File.Exists(_mockFilePath))
        //     return Result<NewsResponse.Get>.NotFound($"Mock data file not found at: {_mockFilePath}");
        //
        // var json = await File.ReadAllTextAsync(_mockFilePath, ctx);
        // var items = JsonSerializer.Deserialize<List<NewsDto.Index>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        var query = dbContext.NewsArticles.AsQueryable();
        var newsArticle = await query.AsNoTracking()
            .Where(n => n.Id == id)
            .Select(n => new NewsDto.Index
            {
                Id = n.Id,
                Title = n.Title,
                Description = n.Description,
                Type = n.Type,
                PublishDate = n.PublishDate,
                Content = n.Content,
                Author = n.Author
            })
            .FirstOrDefaultAsync(ctx);
        if (newsArticle == null)
            return Result<NewsResponse.Get>.NotFound($"News item with id {id} not found.");

        var response = new NewsResponse.Get { NewsArticle = newsArticle };
        return Result.Success(response);
    }
}