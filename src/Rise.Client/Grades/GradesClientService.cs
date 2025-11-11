namespace Rise.Client.Grades;

using Rise.Shared.Grades;
using System.Net.Http.Json;
using System.Text.Json;
using Rise.Shared.Common;


public class GradesClientService(HttpClient httpClient) : IGradesService
{
    public async Task<Result<GradesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<GradesResponse.Index>>($"/api/grades?searchterm={request.SearchTerm}&skip={request.Skip}&take={request.Take}", cancellationToken: ctx);
        return result!;
    }
    public async Task<Result<GradesResponse.Get>> GetGradeByIdAsync(Guid gradeId, CancellationToken ctx = default)
    {
        var response = await httpClient.GetAsync($"/api/grades/{gradeId}", cancellationToken: ctx);
        var result = await response.Content.ReadFromJsonAsync<Result<GradesResponse.Get>>(cancellationToken: ctx);
        return result!;
    }
}