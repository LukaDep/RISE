using Rise.Shared.Identity;

namespace Rise.Server.Endpoints.Grades;

using Rise.Shared.Common;
using Rise.Shared.Grades;

/// <summary>
/// Get a course by its ID.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="gradesService"></param>

public class CourseById(IGradesService gradesService) : EndpointWithoutRequest<Result<GradesResponse.Get>>
{
    public override void Configure()
    {
        Get("/api/grades/{id}");
        Roles(AppRoles.Student);
    }

    public override Task<Result<GradesResponse.Get>> ExecuteAsync(CancellationToken ct)
    {
        var gradeId = Route<Guid>("id");
        return gradesService.GetGradeByIdAsync(gradeId, ct);
    }
}