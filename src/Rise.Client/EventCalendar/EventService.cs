using Rise.Shared.Common;
using Rise.Shared.Events;
using System.Net.Http.Json;
using System.Text.Json;

namespace Rise.Client.EventCalendar
{
    /// <summary>
    /// Client-side service for event calendar operations.
    /// </summary>
    /// <param name="httpClient">The HTTP client for API communication.</param>
    public class EventService(HttpClient httpClient) : IEventService
    {
        /// <summary>
        /// Retrieves a paginated and filtered list of events.
        /// </summary>
        /// <param name="request">The request containing pagination, search, and filter options.</param>
        /// <param name="ctx">Cancellation token.</param>
        /// <returns>A result containing the list of events.</returns>
        public async Task<Result<EventResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
        {
            var result = await httpClient.GetFromJsonAsync<Result<EventResponse.Index>>($"/api/events?searchterm={request.SearchTerm}&skip={request.Skip}&take={request.Take}&filters={JsonSerializer.Serialize(request.Filters)}", cancellationToken: ctx);
            return result!;
        }
    }
}
