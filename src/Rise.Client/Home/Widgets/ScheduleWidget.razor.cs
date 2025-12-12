namespace Rise.Client.Home.Widgets;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.Schedule;

/// <summary>
/// Dashboard widget that displays today's upcoming classes.
/// Shows a compact schedule view with navigation to full schedule.
/// </summary>
public partial class ScheduleWidget : ComponentBase
{
    private List<ScheduleDto.Schedule>? UpcomingClasses { get; set; } = default!;
    private bool _loading;
    private string? _error;
    
    /// <summary>Callback when widget is removed.</summary>
    [Parameter] public EventCallback<Guid> OnRemove { get; set; }
    
    /// <summary>Indicates if edit mode is active.</summary>
    [Parameter] public bool EditMode { get; set; }
    
    /// <summary>Widget index in the grid.</summary>
    [Parameter] public int Index { get; set; }
    
    /// <summary>Unique widget identifier.</summary>
    [Parameter] public Guid WidgetId { get; set; }
    
    /// <summary>Optional start date filter.</summary>
    [Parameter] public DateTime? StartDate { get; set; }
    
    /// <summary>Optional end date filter.</summary>
    [Parameter] public DateTime? EndDate { get; set; }
    
    /// <summary>JavaScript runtime for interop.</summary>
    [Inject] public IJSRuntime Js { get; set; } = default!;
    
    /// <summary>Navigation manager for routing.</summary>
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;
    
    /// <summary>Service for schedule data.</summary>
    [Inject] public IScheduleService ScheduleClientService { get; set; } = default!;

    /// <summary>
    /// Loads today's schedule on initialization.
    /// </summary>
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

    /// <summary>
    /// Navigates to the full schedule page.
    /// </summary>
    private void More()
    {
        NavigationManager.NavigateTo("/schedule");
    }
}