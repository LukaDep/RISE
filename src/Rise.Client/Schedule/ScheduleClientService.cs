using Rise.Shared.Schedule;
using System.Net.Http.Json;
using Rise.Shared.Common;

namespace Rise.Client.Schedule;

public class ScheduleClientService(HttpClient httpClient) : IScheduleService
{
    public async Task<Result<ScheduleDto.Data>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<ScheduleDto.Data>>("/api/schedules", cancellationToken: ctx);
        return result!;
    }
}
