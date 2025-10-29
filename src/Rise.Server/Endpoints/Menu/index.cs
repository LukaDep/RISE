using Rise.Shared.Menu;
using Rise.Shared.Common;

namespace Rise.Server.Endpoints.Menus;

/// <param name="menuService"></param>
public class Index(IMenuService menuService)
    : Endpoint<QueryRequest.SkipTake, Result<MenuResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/Menu");
        AllowAnonymous();
    }

    public override Task<Result<MenuResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return menuService.GetIndexAsync(req, ct);
    }
}
