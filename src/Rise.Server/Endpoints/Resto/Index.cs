using Rise.Shared.Common;
using Rise.Shared.Resto;

namespace Rise.Server.Endpoints.Resto;

public class Index(IRestoService restoService) : Endpoint<QueryRequest.SkipTake, Result<RestoResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/restos");
        AllowAnonymous();
    }

    public override Task<Result<RestoResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return restoService.GetIndexAsync(req, ct);
    }
}
