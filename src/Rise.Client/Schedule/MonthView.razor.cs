using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.Schedule;

namespace Rise.Client.Schedule;

public partial class MonthView : IAsyncDisposable
{
    [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;

    [Parameter] public EventCallback<DateTime> OnDayClick { get; set; }

    private List<ScheduleDto.Schedule>? schedule;

    [Inject] public required IScheduleService ScheduleService { get; set; }
    [Inject] public required IJSRuntime JSRuntime { get; set; }

    private DotNetObjectReference<MonthView>? dotNetRef;
    private string swipeClass = string.Empty;

    private string[] DaysOfWeek = { "Ma", "Di", "Wo", "Do", "Vr", "Za", "Zo" };

    protected override async Task OnInitializedAsync()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 200,
            OrderBy = "Id",
        };

        var result = await ScheduleService.GetIndexAsync(request);
        schedule = result.Value?.Schedules;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            dotNetRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("initSwipe", "monthViewContainer", dotNetRef);
        }
    }

    private async Task AnimateSwipe(string direction)
    {
        swipeClass = direction == "left" ? "swipe-left" : "swipe-right";
        StateHasChanged();
        await Task.Delay(250);
        swipeClass = string.Empty;
        StateHasChanged();
    }

    private async Task GoToToday()
    {
        SelectedDate = DateTime.Today;
        if (OnDayClick.HasDelegate)
        {
            await OnDayClick.InvokeAsync(DateTime.Today);
        }
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
        PreviousMonth();
        StateHasChanged();
    }

    public async Task NextMonthAnimated()
    {
        await AnimateSwipe("left");
        NextMonth();
        StateHasChanged();
    }

    private void PreviousMonth() => SelectedDate = SelectedDate.AddMonths(-1);
    private void NextMonth() => SelectedDate = SelectedDate.AddMonths(1);

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

    private async Task GoToDayView(DateTime date)
    {
        if (OnDayClick.HasDelegate)
        {
            await OnDayClick.InvokeAsync(date);
        }
    }

    public async ValueTask DisposeAsync()
    {
        dotNetRef?.Dispose();
    }
}
