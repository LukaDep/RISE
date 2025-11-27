using Rise.Shared.Schedule;
using System.Net.Http.Json;
using Rise.Shared.Common;

namespace Rise.Client.Schedule;

public class ScheduleClientService(HttpClient httpClient) : IScheduleService
{
    public async Task<Result<ScheduleDto.Data>> GetIndexAsync(QueryRequest.DateRange req, CancellationToken ctx = default)
    {
        var queryParams = new List<string>();
        if (req.StartDate.HasValue)
            queryParams.Add($"StartDate={Uri.EscapeDataString(req.StartDate.Value.ToString("o"))}");
        if (req.EndDate.HasValue)
            queryParams.Add($"EndDate={Uri.EscapeDataString(req.EndDate.Value.ToString("o"))}");
        if (!string.IsNullOrEmpty(req.SearchTerm))
            queryParams.Add($"SearchTerm={Uri.EscapeDataString(req.SearchTerm)}");
        if (req.Skip > 0)
            queryParams.Add($"Skip={req.Skip}");
        if (req.Take != 200)
            queryParams.Add($"Take={req.Take}");
        if (!string.IsNullOrEmpty(req.OrderBy))
            queryParams.Add($"OrderBy={Uri.EscapeDataString(req.OrderBy)}");
        if (req.OrderDescending)
            queryParams.Add($"OrderDescending=true");

        var queryString = string.Join("&", queryParams);
        var url = string.IsNullOrEmpty(queryString) ? "/api/schedules" : $"/api/schedules?{queryString}";

        var result = await httpClient.GetFromJsonAsync<Result<ScheduleDto.Data>>(url, cancellationToken: ctx);
        return result!;
    }
}
