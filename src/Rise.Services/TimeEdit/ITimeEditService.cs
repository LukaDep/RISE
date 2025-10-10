namespace Rise.Shared.TimeEdit;

/// <summary>
/// Provides methods for managing timeedit-related operations.
/// </summary>
public interface ITimeEditService
{
  Task<Result<TimeEditDto.ApiResponse>> GetAsync(TimeEditRequest.Get req, CancellationToken ctx);
}