using Rise.Shared.Common;
using Rise.Shared.Deadlines;
using Rise.Shared.Identity;

namespace Rise.Server.Endpoints.Deadlines;

/// <summary>
/// List all deadlines within a date range.
/// </summary>
/// <param name="deadlineService">The deadline service.</param>
public class Index(IDeadlineService deadlineService) : Endpoint<QueryRequest.DateRange, Result<DeadlineResponse.Index>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/deadlines");
        Roles(AppRoles.Student);
    }

    /// <summary>
    /// Retrieves all deadlines within the specified date range.
    /// </summary>
    /// <param name="req">The date range request containing start and end dates.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the list of deadlines.</returns>
    public override Task<Result<DeadlineResponse.Index>> ExecuteAsync(QueryRequest.DateRange req, CancellationToken ct)
    {
        return deadlineService.GetIndexAsync(req, ct);
    }
}