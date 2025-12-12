using Rise.Shared.News;
using Rise.Shared.Common;

namespace Rise.Server.Endpoints.News;

/// <summary>
/// List all news.
/// </summary>
/// <param name="newsService"></param>
public class Index(INewsService newsService) : Endpoint<QueryRequest.DateRange, Result<NewsResponse.Index>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/news");
        AllowAnonymous();
    }

    /// <summary>
    /// Retrieves all news articles within the specified date range.
    /// </summary>
    /// <param name="req">The date range request containing start and end dates.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the list of news articles.</returns>
    public override Task<Result<NewsResponse.Index>> ExecuteAsync(QueryRequest.DateRange req, CancellationToken ct)
    {
        return newsService.GetIndexAsync(req, ct);
    }
}
