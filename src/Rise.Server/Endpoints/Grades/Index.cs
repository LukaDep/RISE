namespace Rise.Server.Endpoints.Grades;

using Rise.Shared.Common;
using Rise.Shared.Grades;

/// <summary>
/// List all courses.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="gradesService"></param>
public class Index(IGradesService gradesService) : Endpoint<QueryRequest.SkipTake, Result<GradesResponse.CourseList>>
{
    public override void Configure()
    {
        Get("/api/grades/courses");
        AllowAnonymous();
    }

    public override Task<Result<GradesResponse.CourseList>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return gradesService.GetCoursesAsync(req, ct);
    }
}
public class CourseById(IGradesService gradesService) : EndpointWithoutRequest<Result<GradesResponse.CourseById>>
{
    public override void Configure()
    {
        Get("/api/grades/courses/{courseId}");
        AllowAnonymous();
    }

    public override Task<Result<GradesResponse.CourseById>> ExecuteAsync(CancellationToken ct)
    {
        var courseId = Route<string>("courseId");
        return gradesService.GetCourseByIdAsync(courseId, ct);
    }
}