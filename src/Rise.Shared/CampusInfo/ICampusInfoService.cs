using Rise.Shared.Common;

namespace Rise.Shared.CampusInfo;

public interface ICampusInfoService
{
    Task<Result<CampusInfoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
}