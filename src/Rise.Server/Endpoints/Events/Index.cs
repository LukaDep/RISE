using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Server.Endpoints.Events
{
    public class Index(IEventService eventService) : Endpoint<QueryRequest.SkipTake, Result<EventResponse.Index>>
    {
        public override void Configure()
        {
            Get("/api/events");
            AllowAnonymous();
        }
        public override Task<Result<EventResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
        {
            return eventService.GetIndexAsync(req, ct);
        }
    }
}
