namespace Rise.Client.Grades;

using Rise.Shared.Grades;
using System.Net.Http.Json;
using Rise.Shared.Common;


public class GradesClientService(HttpClient httpClient) : IGradesService
{
    public async Task<Result<GradesResponse.CourseList>> GetCoursesAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<GradesResponse.CourseList>>($"/api/grades", cancellationToken: ctx);
        return result!;
    }
    public async Task<Result<GradesResponse.CourseById>> GetCourseByIdAsync(string courseId, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<GradesResponse.CourseById>>($"/api/grades/{courseId}", cancellationToken: ctx);
        return result!;
    }
}