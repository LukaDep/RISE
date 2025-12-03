using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.Schedule;

namespace Rise.Client.Schedule;

public partial class Calendar : ComponentBase
{
    private DateTime SelectedDate = DateTime.Today;
    private List<ScheduleDto.Schedule>? schedule;
    [Inject] public required IScheduleService ScheduleService { get; set; }
    [Inject] public required IJSRuntime JSRuntime { get; set; }
    private DotNetObjectReference<Calendar>? dotNetRef;
    private string swipeClass = string.Empty;
    private List<DateTime> WeekDays => ScheduleHelpers.GetWeekDays(SelectedDate, true);
    [Parameter] public EventCallback<DateTime> SelectedDateChanged { get; set; }
    protected override async Task OnParametersSetAsync()
    {
        await LoadSchedulesAsync();
    }
    private string GetDayAbbreviation(DateTime day)
    {
        return day.ToString("ddd", System.Globalization.CultureInfo.CurrentCulture).Substring(0, 3).ToUpper();
    }

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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            dotNetRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("initSwipe", "calendarContainer", dotNetRef);
        }
    }

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

    private List<ScheduleDto.Schedule> GetSchedulesForDate(DateTime date) =>
        schedule?.Where(r => r.StartDateTime.Date == date.Date).ToList() ?? new List<ScheduleDto.Schedule>();

    private bool HasEventsOnDay(DateTime day) =>
        schedule?.Any(r => r.StartDateTime.Date == day.Date) ?? false;

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

    public async Task PreviousMonthAnimated()
    {
        await AnimateSwipe("right");
        await PreviousMonth();
    }

    public async Task NextMonthAnimated()
    {
        await AnimateSwipe("left");
        await NextMonth();
    }
    private async Task PreviousMonth()
    {
        SelectedDate = SelectedDate.AddMonths(-1);
        await LoadSchedulesAsync();
        StateHasChanged();
    }

    private async Task NextMonth()
    {
        SelectedDate = SelectedDate.AddMonths(1);
        await LoadSchedulesAsync();
        StateHasChanged();
    }

    private async Task ChangeMonth(int offset)
    {
        var newDate = SelectedDate.AddMonths(offset);
        SelectedDate = newDate;
        await SelectedDateChanged.InvokeAsync(newDate);
    }

    [JSInvokable]
    public async Task SwipeNext()
    {
        await NextMonthAnimated();
    }

    [JSInvokable]
    public async Task SwipePrevious()
    {
        await PreviousMonthAnimated();
    }

    private void GoToDayView(DateTime day)
    {
        Navigation.NavigateTo($"/schedule/{day:yyyy-MM-dd}");
    }

    public async ValueTask DisposeAsync()
    {
        dotNetRef?.Dispose();
    }
}
