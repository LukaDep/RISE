using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Timers;

namespace Rise.Client.Schedule;

public partial class Schedule : IAsyncDisposable
{
    [Parameter] public EventCallback<DateTime> OnDayClick { get; set; }
    [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;
    [Parameter] public EventCallback<DateTime> OnDateChanged { get; set; }

    [Inject] public required IJSRuntime JSRuntime { get; set; }

    private DotNetObjectReference<Schedule>? dotNetRef;
    private string swipeClass = string.Empty;
    private System.Timers.Timer? currentTimeTimer;
    private DateTime currentTime = DateTime.Now;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        SelectedDate = GetNextWeekdayIfWeekend(SelectedDate);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            dotNetRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("initSwipe", "weekViewContainer", dotNetRef);

            StartCurrentTimeTimer();
        }
    }

    private void StartCurrentTimeTimer()
    {
        currentTimeTimer = new System.Timers.Timer(60000);
        currentTimeTimer.Elapsed += OnTimerElapsed;
        currentTimeTimer.AutoReset = true;
        currentTimeTimer.Start();
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        currentTime = DateTime.Now;
        InvokeAsync(StateHasChanged);
    }

    private async Task AnimateSwipe(string direction)
    {
        swipeClass = direction == "left" ? "swipe-left" : "swipe-right";
        StateHasChanged();
        await Task.Delay(300);
        swipeClass = string.Empty;
        StateHasChanged();
    }

    private DateTime WeekStartDate => ScheduleHelpers.GetWeekStart(SelectedDate);

    private int WeekNumber => ScheduleHelpers.GetWeekNumber(WeekStartDate);

    private List<DateTime> WeekDays => ScheduleHelpers.GetWeekDays(SelectedDate);

    public async Task GoToToday()
    {
        var today = GetNextWeekdayIfWeekend(DateTime.Today);
        SelectedDate = today;
        if (OnDayClick.HasDelegate)
        {
            await OnDayClick.InvokeAsync(today);
        }
        await OnDateChanged.InvokeAsync(SelectedDate);
        StateHasChanged();
    }

    private async Task SelectDay(DateTime day)
    {
        var selectedDay = GetNextWeekdayIfWeekend(day);
        SelectedDate = selectedDay;
        if (OnDayClick.HasDelegate)
        {
            await OnDayClick.InvokeAsync(selectedDay);
        }
        await OnDateChanged.InvokeAsync(selectedDay);
        StateHasChanged();
    }

    private async Task HandleDateChanged(DateTime newDate)
    {
        var adjustedDate = GetNextWeekdayIfWeekend(newDate);
        SelectedDate = adjustedDate;
        await OnDateChanged.InvokeAsync(adjustedDate);
        StateHasChanged();
    }

    private static DateTime GetNextWeekdayIfWeekend(DateTime date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Saturday => date.AddDays(2),
            DayOfWeek.Sunday => date.AddDays(1),
            _ => date
        };
    }

    public async Task PreviousWeekAnimated()
    {
        await AnimateSwipe("right");
        PreviousWeek();
        StateHasChanged();
    }

    public async Task NextWeekAnimated()
    {
        await AnimateSwipe("left");
        NextWeek();
        StateHasChanged();
    }

    private async Task PreviousWeek() => await OnDateChanged.InvokeAsync(SelectedDate.AddDays(-7));
    private async Task NextWeek() => await OnDateChanged.InvokeAsync(SelectedDate.AddDays(7));

    [JSInvokable]
    public async Task SwipeNext()
    {
        await NextWeekAnimated();
    }

    [JSInvokable]
    public async Task SwipePrevious()
    {
        await PreviousWeekAnimated();
    }

    public async ValueTask DisposeAsync()
    {
        if (currentTimeTimer != null)
        {
            currentTimeTimer.Stop();
            currentTimeTimer.Elapsed -= OnTimerElapsed;
            currentTimeTimer.Dispose();
        }
        dotNetRef?.Dispose();
    }
}
