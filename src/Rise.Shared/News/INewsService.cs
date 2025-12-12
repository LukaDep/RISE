using Rise.Shared.Common;

namespace Rise.Shared.News;

/// <summary>
/// Service interface for managing news articles.
/// </summary>
public interface INewsService
{
    /// <summary>
    /// Retrieves a filtered and paginated list of news articles.
    /// Supports searching by title and author, sorting, and filtering by date range.
    /// </summary>
    /// <param name="request">QueryRequest.DateRange with SearchTerm, OrderBy, OrderDescending, StartDate, EndDate, Skip and Take</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with NewsResponse.Index containing the list of news articles and the total count</returns>
    Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default);

    /// <summary>
    /// Retrieves a specific news article by ID.
    /// </summary>
    /// <param name="id">The Guid of the news article to retrieve</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with NewsResponse.Get containing the news article, or NotFound if the article does not exist</returns>
    Task<Result<NewsResponse.Get>> GetByIdAsync(Guid id, CancellationToken ctx = default);
}