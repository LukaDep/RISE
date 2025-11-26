using Rise.Shared.Common;
using Rise.Shared.Events;
using System.Net.Http.Json;
using System.Text.Json;

namespace Rise.Client.EventCalendar
{
    public class EventService(HttpClient httpClient) : IEventService
    {
        public async Task<Result<EventResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
        {
            var result = await httpClient.GetFromJsonAsync<Result<EventResponse.Index>>($"/api/events?searchterm={request.SearchTerm}&skip={request.Skip}&take={request.Take}&filters={JsonSerializer.Serialize(request.Filters)}", cancellationToken: ctx);
            return result!;
        }
    }
}
