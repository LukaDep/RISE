using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.Schedule;

namespace Rise.Client.Schedule;

/// <summary>
/// Code-behind for the Calendar page component.
/// Displays a monthly calendar view with scheduled events.
/// </summary>
public partial class Calendar : ComponentBase
{
    private DateTime SelectedDate = DateTime.Today;
    private List<ScheduleDto.Schedule>? schedule;
    
    /// <summary>Service for schedule data operations.</summary>
    [Inject] public required IScheduleService ScheduleService { get; set; }
    
    /// <summary>JavaScript runtime for interop calls.</summary>
    [Inject] public required IJSRuntime JSRuntime { get; set; }
    
    private DotNetObjectReference<Calendar>? dotNetRef;
    private string swipeClass = string.Empty;
    
    /// <summary>Gets the days of the current week.</summary>
    private List<DateTime> WeekDays => ScheduleHelpers.GetWeekDays(SelectedDate, true);
    
    /// <summary>Event callback when selected date changes.</summary>
    [Parameter] public EventCallback<DateTime> SelectedDateChanged { get; set; }
    
    /// <summary>
    /// Loads schedule data when parameters change.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        await LoadSchedulesAsync();
    }
    
    /// <summary>
    /// Gets the abbreviated day name for display.
    /// </summary>
    private string GetDayAbbreviation(DateTime day)
    {
        return day.ToString("ddd", System.Globalization.CultureInfo.CurrentCulture).ToUpper();
    }

    /// <summary>
    /// Loads schedules for the current month.
    /// </summary>
    private async Task LoadSchedulesAsync()
    {
        var start = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
        var end = start.AddMonths(1).AddTicks(-1);

        QueryRequest.DateRange request = new()
        {
            Skip = 0,
            Take = 200,
            StartDate = start,
            EndDate = end
        };

        var result = await ScheduleService.GetIndexAsync(request);
        schedule = result.Value?.Schedules;
    }

    /// <summary>
    /// Initializes swipe gestures after first render.
    /// </summary>
    /// <param name="firstRender">True if this is the first render.</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            dotNetRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("initSwipe", "calendarContainer", dotNetRef);
        }
    }

    /// <summary>
    /// Animates a swipe transition in the specified direction.
    /// </summary>
    /// <param name="direction">Direction of swipe ("left" or "right").</param>
    private async Task AnimateSwipe(string direction)
    {
        swipeClass = direction == "left" ? "swipe-left" : "swipe-right";
        StateHasChanged();
        await Task.Delay(300);
        swipeClass = string.Empty;
        StateHasChanged();
    }

    private DateTime FirstDayOfMonth => new DateTime(SelectedDate.Year, SelectedDate.Month, 1);

    private List<DateTime> MonthDays
    {
        get
        {
            var firstDay = FirstDayOfMonth;
            var daysFromPrevMonth = ((int)firstDay.DayOfWeek + 6) % 7; // Monday = 0
            var startDate = firstDay.AddDays(-daysFromPrevMonth);

            return Enumerable.Range(0, 42) // 6 weeks * 7 days
                             .Select(i => startDate.AddDays(i))
                             .ToList();
        }
    }

    /// <summary>
    /// Gets schedules for a specific date.
    /// </summary>
    /// <param name="date">The date to get schedules for.</param>
    /// <returns>List of schedules on that date.</returns>
    private List<ScheduleDto.Schedule> GetSchedulesForDate(DateTime date) =>
        schedule?.Where(r => r.StartDateTime.Date == date.Date).ToList() ?? new List<ScheduleDto.Schedule>();

    /// <summary>
    /// Checks if a day has scheduled events.
    /// </summary>
    /// <param name="day">The day to check.</param>
    /// <returns>True if the day has events.</returns>
    private bool HasEventsOnDay(DateTime day) =>
        schedule?.Any(r => r.StartDateTime.Date == day.Date) ?? false;

    /// <summary>
    /// Gets distinct work forms for the current month.
    /// </summary>
    /// <returns>List of unique work form names.</returns>
    private List<string> GetWorkFormsInMonth()
    {
        var currentMonthStart = FirstDayOfMonth;
        var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

        var workForms = schedule?
            .Where(r => r.StartDateTime.Date >= currentMonthStart && r.StartDateTime.Date <= currentMonthEnd)
            .Select(r => r.WorkForm)
            .Distinct()
            .OrderBy(w => w)
            .ToList() ?? new List<string>();

        return workForms;
    }

    /// <summary>
    /// Navigates to previous month with animation.
    /// </summary>
    public async Task PreviousMonthAnimated()
    {
        await AnimateSwipe("right");
        await PreviousMonth();
    }

    /// <summary>
    /// Navigates to next month with animation.
    /// </summary>
    public async Task NextMonthAnimated()
    {
        await AnimateSwipe("left");
        await NextMonth();
    }
    
    /// <summary>
    /// Navigates to the previous month.
    /// </summary>
    private async Task PreviousMonth()
    {
        SelectedDate = SelectedDate.AddMonths(-1);
        await LoadSchedulesAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Navigates to the next month.
    /// </summary>
    private async Task NextMonth()
    {
        SelectedDate = SelectedDate.AddMonths(1);
        await LoadSchedulesAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Changes the month by the specified offset.
    /// </summary>
    /// <param name="offset">Number of months to offset (negative for past).</param>
    private async Task ChangeMonth(int offset)
    {
        var newDate = SelectedDate.AddMonths(offset);
        SelectedDate = newDate;
        await SelectedDateChanged.InvokeAsync(newDate);
    }

    /// <summary>
    /// JavaScript invokable method for swipe next gesture.
    /// </summary>
    [JSInvokable]
    public async Task SwipeNext()
    {
        await NextMonthAnimated();
    }

    /// <summary>
    /// JavaScript invokable method for swipe previous gesture.
    /// </summary>
    [JSInvokable]
    public async Task SwipePrevious()
    {
        await PreviousMonthAnimated();
    }

    /// <summary>
    /// Navigates to the day view for a specific day.
    /// </summary>
    /// <param name="day">The day to navigate to.</param>
    private void GoToDayView(DateTime day)
    {
        Navigation.NavigateTo($"/schedule/{day:yyyy-MM-dd}");
    }

    /// <summary>
    /// Disposes the JS interop reference.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        dotNetRef?.Dispose();
    }
}
