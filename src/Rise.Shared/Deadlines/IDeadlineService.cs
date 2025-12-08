using Rise.Shared.Common;

namespace Rise.Shared.Deadlines
{
    public interface IDeadlineService
    {
        Task<Result<DeadlineResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default);

    }
}