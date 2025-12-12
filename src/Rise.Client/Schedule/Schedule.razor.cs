using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Timers;

namespace Rise.Client.Schedule;

/// <summary>
/// Code-behind for the Schedule page component.
/// Displays a weekly schedule view with swipe navigation.
/// </summary>
public partial class Schedule : IAsyncDisposable
{
    /// <summary>Event callback when a day is clicked.</summary>
    [Parameter] public EventCallback<DateTime> OnDayClick { get; set; }
    
    /// <summary>The currently selected date.</summary>
    [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;
    
    /// <summary>Event callback when date changes.</summary>
    [Parameter] public EventCallback<DateTime> OnDateChanged { get; set; }

    /// <summary>JavaScript runtime for interop calls.</summary>
    [Inject] public required IJSRuntime JSRuntime { get; set; }

    private DotNetObjectReference<Schedule>? dotNetRef;
    private string swipeClass = string.Empty;
    private System.Timers.Timer? currentTimeTimer;
    private DateTime currentTime = DateTime.Now;

    /// <summary>
    /// Handles parameter changes, adjusting weekends to next weekday.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        SelectedDate = GetNextWeekdayIfWeekend(SelectedDate);
    }

    /// <summary>
    /// Initializes swipe gestures and timer after first render.
    /// </summary>
    /// <param name="firstRender">True if this is the first render.</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            dotNetRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("initSwipe", "weekViewContainer", dotNetRef);

            StartCurrentTimeTimer();
        }
    }

    /// <summary>
    /// Starts the timer to update current time indicator.
    /// </summary>
    private void StartCurrentTimeTimer()
    {
        currentTimeTimer = new System.Timers.Timer(60000);
        currentTimeTimer.Elapsed += OnTimerElapsed;
        currentTimeTimer.AutoReset = true;
        currentTimeTimer.Start();
    }

    /// <summary>
    /// Handles timer elapsed event to update current time.
    /// </summary>
    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        currentTime = DateTime.Now;
        InvokeAsync(StateHasChanged);
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

    private DateTime WeekStartDate => ScheduleHelpers.GetWeekStart(SelectedDate);

    private int WeekNumber => ScheduleHelpers.GetWeekNumber(WeekStartDate);

    private List<DateTime> WeekDays => ScheduleHelpers.GetWeekDays(SelectedDate);

    /// <summary>
    /// Navigates to today's schedule.
    /// </summary>
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

    /// <summary>
    /// Selects a specific day in the schedule.
    /// </summary>
    /// <param name="day">The day to select.</param>
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

    /// <summary>
    /// Handles date changed events, adjusting for weekends.
    /// </summary>
    /// <param name="newDate">The new date.</param>
    private async Task HandleDateChanged(DateTime newDate)
    {
        var adjustedDate = GetNextWeekdayIfWeekend(newDate);
        SelectedDate = adjustedDate;
        await OnDateChanged.InvokeAsync(adjustedDate);
        StateHasChanged();
    }

    /// <summary>
    /// Adjusts a weekend date to the next weekday.
    /// </summary>
    /// <param name="date">The date to adjust.</param>
    /// <returns>The same date if weekday, or next Monday if weekend.</returns>
    private static DateTime GetNextWeekdayIfWeekend(DateTime date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Saturday => date.AddDays(2),
            DayOfWeek.Sunday => date.AddDays(1),
            _ => date
        };
    }

    /// <summary>
    /// Navigates to the previous week with swipe animation.
    /// </summary>
    public async Task PreviousWeekAnimated()
    {
        await AnimateSwipe("right");
        PreviousWeek();
        StateHasChanged();
    }

    /// <summary>
    /// Navigates to the next week with swipe animation.
    /// </summary>
    public async Task NextWeekAnimated()
    {
        await AnimateSwipe("left");
        NextWeek();
        StateHasChanged();
    }

    /// <summary>
    /// Navigates to the previous week by subtracting 7 days.
    /// </summary>
    private async Task PreviousWeek() => await OnDateChanged.InvokeAsync(SelectedDate.AddDays(-7));

    /// <summary>
    /// Navigates to the next week by adding 7 days.
    /// </summary>
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
