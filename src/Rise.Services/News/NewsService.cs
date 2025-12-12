using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Services.News;

/// <summary>
/// Service for managing news articles.
/// </summary>
public class NewsService(ApplicationDbContext dbContext) : INewsService
{
    /// <summary>
    /// Retrieves a filtered and paginated list of news articles.
    /// Supports searching by title and author, sorting, and filtering by date range.
    /// </summary>
    /// <param name="request">QueryRequest.DateRange with SearchTerm, OrderBy, OrderDescending, StartDate, EndDate, Skip and Take</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with NewsResponse.Index containing the list of news articles and the total count</returns>
    public async Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
    {
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
            query = query.OrderByDescending(p => p.PublishDate);
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
                Author = n.Author,
                ImageUrl = n.ImageUrl
            })
            .ToListAsync(ctx);



        return Result.Success(new NewsResponse.Index
        {
            News = news,
            TotalCount = totalCount,
        }
        );
    }

    /// <summary>
    /// Retrieves a specific news article by ID.
    /// </summary>
    /// <param name="id">The Guid of the news article to retrieve</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with NewsResponse.Get containing the news article, or NotFound if the article does not exist</returns>
    public async Task<Result<NewsResponse.Get>> GetByIdAsync(Guid id, CancellationToken ctx = default)
    {
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
                Author = n.Author,
                ImageUrl = n.ImageUrl
            })
            .FirstOrDefaultAsync(ctx);
        if (newsArticle == null)
            return Result<NewsResponse.Get>.NotFound($"News item with id {id} not found.");

        var response = new NewsResponse.Get { NewsArticle = newsArticle };
        return Result.Success(response);
    }
}
