using Rise.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Events
{
    /// <summary>
    /// Service interface for managing events.
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Retrieves a filtered and paginated list of events.
        /// Supports searching by type and title, sorting, and filtering by event type.
        /// </summary>
        /// <param name="request">QueryRequest.SkipTake with SearchTerm, OrderBy, Filters, Skip and Take</param>
        /// <param name="ctx">CancellationToken to cancel the operation</param>
        /// <returns>Result with EventResponse.Index containing the list of events</returns>
        Task<Result<EventResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
    }
}
