namespace Rise.Shared.Widgets;

public interface IWidgetService
{
    Task<Result<WidgetResponse.Index>> GetIndexByUserIdAsync(CancellationToken ctx = default);

    Task<Result> UpdateUserWidgetsAsync(WidgetRequest.Update request, CancellationToken ctx = default);
}