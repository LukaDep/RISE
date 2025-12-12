using Rise.Shared.Common;
using Rise.Shared.Campus;
namespace Rise.Server.Endpoints.Campus;

/// <summary>
/// List all campus plans.
/// </summary>
/// <param name="campusService">The campus service.</param>
public class Index(ICampusService campusService) : Endpoint<QueryRequest.SkipTake, Result<CampusResponse.Index>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/campuses");
        AllowAnonymous();
    }

    /// <summary>
    /// Retrieves a paginated list of all campuses.
    /// </summary>
    /// <param name="req">The pagination request containing skip and take values.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the list of campuses.</returns>
    public override Task<Result<CampusResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return campusService.GetIndexAsync(req, ct);
    }
}