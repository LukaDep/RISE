using Rise.Shared.Common;
using Rise.Shared.Resto;

namespace Rise.Server.Endpoints.Resto;

/// <summary>
/// List all restaurants.
/// </summary>
/// <param name="restoService">The resto service.</param>
public class Index(IRestoService restoService) : Endpoint<QueryRequest.SkipTake, Result<RestoResponse.Index>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/restos");
        AllowAnonymous();
    }

    /// <summary>
    /// Retrieves a paginated list of all restaurants.
    /// </summary>
    /// <param name="req">The pagination request containing skip and take values.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the list of restaurants.</returns>
    public override Task<Result<RestoResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return restoService.GetIndexAsync(req, ct);
    }
}
