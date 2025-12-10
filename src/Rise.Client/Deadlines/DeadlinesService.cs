using Microsoft.AspNetCore.WebUtilities;
using Rise.Shared.Common;
using Rise.Shared.Deadlines;
using Rise.Shared.Schedule;
using System.Net;
using System.Net.Http.Json;


public class DeadlinesService(HttpClient httpClient) : IDeadlineService
{
    public async Task<Result<DeadlineResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["skip"] = request.Skip.ToString(),
            ["take"] = request.Take.ToString()
        };

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            parameters["searchterm"] = request.SearchTerm!;

        if (request.StartDate.HasValue)
            parameters["startdate"] = request.StartDate.Value.ToString("O");
        if (request.EndDate.HasValue)
            parameters["enddate"] = request.EndDate.Value.ToString("O");

        var url = QueryHelpers.AddQueryString("/api/deadlines", parameters);

        var response = await httpClient.GetAsync(url, cancellationToken: ctx);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Result<DeadlineResponse.Index>.Error("Not Authorized");
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Result<DeadlineResponse.Index>>(cancellationToken: ctx);
        return result!;
    }

}

