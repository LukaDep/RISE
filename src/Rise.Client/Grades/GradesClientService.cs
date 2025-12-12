using System.Net;


namespace Rise.Client.Grades;

using Rise.Shared.Grades;
using System.Net.Http.Json;
using System.Text.Json;
using Rise.Shared.Common;

/// <summary>
/// Client-side service for grades and course operations.
/// </summary>
/// <param name="httpClient">The HTTP client for API communication.</param>
public class GradesClientService(HttpClient httpClient) : IGradesService
{
    /// <summary>
    /// Retrieves a paginated list of all courses with grades.
    /// </summary>
    /// <param name="request">The pagination request.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result containing the list of courses, or an error if unauthorized.</returns>
    public async Task<Result<GradesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var response = await httpClient.GetAsync($"/api/grades?searchterm={request.SearchTerm}&skip={request.Skip}&take={request.Take}", cancellationToken: ctx);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Result<GradesResponse.Index>.Error("Not Authorized");
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Result<GradesResponse.Index>>(cancellationToken: ctx);
        return result!;
    }

    /// <summary>
    /// Retrieves a specific course with its grades by course ID.
    /// </summary>
    /// <param name="gradeId">The course/grade ID.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result containing the course grade details, or an error if unauthorized.</returns>
    public async Task<Result<GradesResponse.Get>> GetGradeByIdAsync(Guid gradeId, CancellationToken ctx = default)
    {
        var response = await httpClient.GetAsync($"/api/grades/{gradeId}", cancellationToken: ctx);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Result<GradesResponse.Get>.Error("Not Authorized");
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Result<GradesResponse.Get>>(cancellationToken: ctx);
        return result!;
    }
}