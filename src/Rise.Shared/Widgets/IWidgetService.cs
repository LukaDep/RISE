namespace Rise.Shared.Widgets;

/// <summary>
/// Service interface for managing user widgets.
/// </summary>
public interface IWidgetService
{
    /// <summary>
    /// Retrieves all widgets for the current user.
    /// Returns an empty list if the user is not logged in.
    /// </summary>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with WidgetResponse.Index containing the list of UserWidgets</returns>
    Task<Result<WidgetResponse.Index>> GetIndexByUserIdAsync(CancellationToken ctx = default);

    /// <summary>
    /// Updates the user's widgets based on the provided request.
    /// Updates existing widgets, removes missing widgets, and adds new ones.
    /// Validates that the user owns the widgets being modified.
    /// </summary>
    /// <param name="request">WidgetRequest.Update with the list of UserWidgets to update</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result.Success on successful update, Result.Forbidden when attempting to modify widgets owned by others, Result.NotFound for unknown widget types</returns>
    Task<Result> UpdateUserWidgetsAsync(WidgetRequest.Update request, CancellationToken ctx = default);
}