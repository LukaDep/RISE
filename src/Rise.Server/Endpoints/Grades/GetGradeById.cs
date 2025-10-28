namespace Rise.Server.Endpoints.Grades;

using Rise.Shared.Common;
using Rise.Shared.Grades;

/// <summary>
/// Get a course by its ID.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="gradesService"></param>

public class CourseById(IGradesService gradesService) : EndpointWithoutRequest<Result<GradesResponse.CourseById>>
{
    public override void Configure()
    {
        Get("/api/grades/{courseId}");
        AllowAnonymous();
    }

    public override Task<Result<GradesResponse.CourseById>> ExecuteAsync(CancellationToken ct)
    {
        var courseId = Route<string>("courseId");
        return gradesService.GetCourseByIdAsync(courseId, ct);
    }
}