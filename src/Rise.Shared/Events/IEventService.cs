using Rise.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Events
{
    public interface IEventService
    {
        Task<Result<EventResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
    }
}
