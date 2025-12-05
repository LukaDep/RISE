using Rise.Shared.Identity;

namespace Rise.Server.Endpoints.Grades;

using Rise.Shared.Common;
using Rise.Shared.Grades;

/// <summary>
/// List all courses.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="gradesService"></param>
public class Index(IGradesService gradesService) : Endpoint<QueryRequest.SkipTake, Result<GradesResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/grades");
        Roles(AppRoles.Student);
    }

    public override Task<Result<GradesResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return gradesService.GetIndexAsync(req, ct);
    }
}