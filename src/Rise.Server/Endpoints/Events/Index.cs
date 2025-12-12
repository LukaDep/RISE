using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Server.Endpoints.Events
{
    /// <summary>
    /// List all events.
    /// </summary>
    /// <param name="eventService">The event service.</param>
    public class Index(IEventService eventService) : Endpoint<QueryRequest.SkipTake, Result<EventResponse.Index>>
    {
        /// <summary>
        /// Configures the endpoint route and authorization.
        /// </summary>
        public override void Configure()
        {
            Get("/api/events");
            AllowAnonymous();
        }

        /// <summary>
        /// Retrieves a paginated list of all events.
        /// </summary>
        /// <param name="req">The pagination request containing skip and take values.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A result containing the list of events.</returns>
        public override Task<Result<EventResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
        {
            return eventService.GetIndexAsync(req, ct);
        }
    }
}
