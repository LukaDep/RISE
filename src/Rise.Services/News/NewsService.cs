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
    public async Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
    {
        // //read from mock json file
        // if (!File.Exists(_mockFilePath))
        //     return Result<NewsResponse.Index>.NotFound($"Mock data file not found at: {_mockFilePath}");
        // var json = await File.ReadAllTextAsync(_mockFilePath, ctx);
        //
        // // Deserialize the JSON data into a list of NewsDto.Index
        // var query = JsonSerializer.Deserialize<List<NewsDto.Index>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        var query = dbContext.NewsItems.AsQueryable();



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
        // --- Robust date handling: treat default(DateTime) as "not provided" ---
        DateTime? start = request.StartDate;
        DateTime? end = request.EndDate;

        if (start.HasValue && start.Value == default(DateTime))
            start = null;
        if (end.HasValue && end.Value == default(DateTime))
            end = null;

        if (start.HasValue && end.HasValue)
        {
            var s = start.Value.Date;
            var e = end.Value.Date;
            if (s > e)
                (s, e) = (e, s);

            query = query.Where(n => n.PublishDate.Date >= s && n.PublishDate.Date <= e);
        }
        else if (start.HasValue)
        {
            var s = start.Value.Date;
            query = query.Where(n => n.PublishDate.Date >= s);
        }
        else if (end.HasValue)
        {
            var e = end.Value.Date;
            query = query.Where(n => n.PublishDate.Date <= e);
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

    public async Task<Result<NewsResponse.Get>> GetByIdAsync(int id, CancellationToken ctx = default)
    {
        // if (!File.Exists(_mockFilePath))
        //     return Result<NewsResponse.Get>.NotFound($"Mock data file not found at: {_mockFilePath}");
        //
        // var json = await File.ReadAllTextAsync(_mockFilePath, ctx);
        // var items = JsonSerializer.Deserialize<List<NewsDto.Index>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        var query = dbContext.NewsItems.AsQueryable();
        var newsItem = await query.AsNoTracking()
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
        if (newsItem == null)
            return Result<NewsResponse.Get>.NotFound($"News item with id {id} not found.");

        var response = new NewsResponse.Get { NewsItem = newsItem };
        return Result.Success(response);
    }
}
