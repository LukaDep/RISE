using Rise.Shared.Common;
using Rise.Shared.Campus;
namespace Rise.Server.Endpoints.Campus;

// <summary>
// List all campus plans.
// See https://fast-endpoints.com/
// </summary>
// <param name="campusService"></param>
public class Index(ICampusService campusService) : Endpoint<QueryRequest.SkipTake, Result<CampusResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/campuses");
        AllowAnonymous();
    }

    public override Task<Result<CampusResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return campusService.GetIndexAsync(req, ct);
    }
}