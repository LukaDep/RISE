using Rise.Shared.Identity;

namespace Rise.Server.Endpoints.Grades;

using Rise.Shared.Common;
using Rise.Shared.Grades;

/// <summary>
/// List all courses.
/// </summary>
/// <param name="gradesService"></param>
public class Index(IGradesService gradesService) : Endpoint<QueryRequest.SkipTake, Result<GradesResponse.Index>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/grades");
        Roles(AppRoles.Student);
    }

    /// <summary>
    /// Retrieves a paginated list of all courses with grades for the current user.
    /// </summary>
    /// <param name="req">The pagination request containing skip and take values.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the list of courses.</returns>
    public override Task<Result<GradesResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return gradesService.GetIndexAsync(req, ct);
    }
}