using Rise.Shared.Menus;
using Rise.Shared.Common;

namespace Rise.Server.Endpoints.Menus;

/// <summary>
/// List all menus.
/// Zie https://fast-endpoints.com/
/// </summary>
/// <param name="menuService"></param>
public class Index(IMenuService menuService)
    : Endpoint<QueryRequest.SkipTake, Result<MenuResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/menus");
        AllowAnonymous();
    }

    public override Task<Result<MenuResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return menuService.GetIndexAsync(req, ct);
    }
}
