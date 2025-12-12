using System;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Server.Endpoints.News;

/// <summary>
/// Get a news article by ID.
/// </summary>
/// <param name="newsService">The news service.</param>
public class Get(INewsService newsService) : EndpointWithoutRequest<Result<NewsResponse.Get>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/news/{id}");
        AllowAnonymous();
    }

    /// <summary>
    /// Retrieves a specific news article by its unique identifier.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the news article details.</returns>
    public override async Task<Result<NewsResponse.Get>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        return await newsService.GetByIdAsync(id, ct);
    }
}
