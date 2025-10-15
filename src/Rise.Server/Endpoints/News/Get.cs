using System;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Server.Endpoints.News;

public class Get(INewsService newsService) : EndpointWithoutRequest<Result<NewsResponse.Get>>
{
  public override void Configure()
  {
    Get("/api/news/{id}");
    AllowAnonymous();
  }

  public override async Task<Result<NewsResponse.Get>> ExecuteAsync(CancellationToken ct)
  {
    var id = Route<int>("id");
    return await newsService.GetByIdAsync(id, ct);
  }
}
