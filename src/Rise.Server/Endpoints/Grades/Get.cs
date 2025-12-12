using Rise.Shared.Identity;

namespace Rise.Server.Endpoints.Grades;

using Rise.Shared.Common;
using Rise.Shared.Grades;

/// <summary>
/// Get a course by its ID.
/// </summary>
/// <param name="gradesService"></param>

public class CourseById(IGradesService gradesService) : EndpointWithoutRequest<Result<GradesResponse.Get>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/grades/{id}");
        Roles(AppRoles.Student);
    }

    /// <summary>
    /// Retrieves a specific course with its grades by course ID.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the course grade details.</returns>
    public override Task<Result<GradesResponse.Get>> ExecuteAsync(CancellationToken ct)
    {
        var gradeId = Route<Guid>("id");
        return gradesService.GetGradeByIdAsync(gradeId, ct);
    }
}