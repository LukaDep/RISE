using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.Schedule;

namespace Rise.Client.Home.Widgets;

public partial class ScheduleWidget : ComponentBase
{
    private List<ScheduleDto.Schedule>? UpcomingClasses { get; set; } = default!;
    private bool _loading;
    private string? _error;
    [Parameter] public EventCallback<Guid> OnRemove { get; set; }
    [Parameter] public bool EditMode { get; set; }
    [Parameter] public int Index { get; set; }
    [Parameter] public Guid WidgetId { get; set; }
    [Inject] public IJSRuntime Js { get; set; } = default!;
    [Parameter] public DateTime? StartDate { get; set; }
    [Parameter] public DateTime? EndDate { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        try
        {
            QueryRequest.DateRange request = new()
            {
                Skip = 0,
                Take = 200,
                StartDate = StartDate,
                EndDate = EndDate
            };

            var resultClasses = await ScheduleClientService.GetIndexAsync(request);
            UpcomingClasses = resultClasses
                .Value?
                .Schedules
                .Where(r => r.StartDateTime.Date.DayOfYear == DateTime.Now.DayOfYear)
                .OrderBy(r => r.StartDateTime)
                .ToList();
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _loading = false;
            StateHasChanged();
        }
    }

    private void More()
    {
        NavigationManager.NavigateTo("/schedule");
    }
}