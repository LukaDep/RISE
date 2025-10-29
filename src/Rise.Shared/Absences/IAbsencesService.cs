using Rise.Shared.Common;

namespace Rise.Shared.Absences;

public interface IAbsencesService
{
    Task<Result<AbsencesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);

}