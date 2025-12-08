using Rise.Shared.Common;
using Rise.Shared.Deadlines;
using Rise.Shared.Identity;

namespace Rise.Server.Endpoints.Deadlines;

public class Index(IDeadlineService deadlineService) : Endpoint<QueryRequest.DateRange, Result<DeadlineResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/deadlines");
        Roles(AppRoles.Student);
    }

    public override Task<Result<DeadlineResponse.Index>> ExecuteAsync(QueryRequest.DateRange req, CancellationToken ct)
    {
        return deadlineService.GetIndexAsync(req, ct);
    }
}