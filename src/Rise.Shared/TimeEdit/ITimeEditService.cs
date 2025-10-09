namespace Rise.Shared.TimeEdit;

/// <summary>
/// Provides methods for managing timeedit-related operations.
/// </summary>
public interface ITimeEditService
{
  Task<Result> EditAsync(ProjectRequest.Edit req, CancellationToken ctx);
}