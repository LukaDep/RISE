using Rise.Shared.News;
using Rise.Shared.Common;

namespace Rise.Server.Endpoints.News;

/// <summary>
/// List all products.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="newsService"></param>
public class CampusInfo(INewsService newsService) : Endpoint<QueryRequest.SkipTake, Result<NewsResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/news");
        AllowAnonymous();
    }

    public override Task<Result<NewsResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return newsService.GetIndexAsync(req, ct);
    }
}
