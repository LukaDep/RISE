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

    private string GetWeekRangeTitle()
    {
        var weekStart = ScheduleHelpers.GetWeekStart(SelectedDate);
        return $"{weekStart.ToString("d MMM", System.Globalization.CultureInfo.CurrentCulture)} – {weekStart.AddDays(6).ToString("d MMM", System.Globalization.CultureInfo.CurrentCulture)}";
    }

    private DateTime WeekStartDate => ScheduleHelpers.GetWeekStart(SelectedDate);

    private int WeekNumber => ScheduleHelpers.GetWeekNumber(WeekStartDate);

    private List<DateTime> WeekDays => ScheduleHelpers.GetWeekDays(SelectedDate);

    public async Task GoToToday()
    {
        SelectedDate = DateTime.Today;
        if (OnDayClick.HasDelegate)
        {
            await OnDayClick.InvokeAsync(DateTime.Today);
        }
        await OnDateChanged.InvokeAsync(SelectedDate);
        StateHasChanged();
    }

    private async Task SelectDay(DateTime day)
    {
        SelectedDate = day;
        if (OnDayClick.HasDelegate)
        {
            await OnDayClick.InvokeAsync(day);
        }
        await OnDateChanged.InvokeAsync(day);
        StateHasChanged();
    }

    private async Task HandleDateChanged(DateTime newDate)
    {
        SelectedDate = newDate;
        await OnDateChanged.InvokeAsync(newDate);
        StateHasChanged();
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

    private int GetCurrentDayIndex()
    {
        var today = DateTime.Today;
        var weekStart = WeekStartDate;

        for (int i = 0; i < 5; i++)
        {
            if (weekStart.AddDays(i).Date == today)
                return i;
        }

        return -1;
    }


    private string GetLocalizedDayName(DateTime day) => day.DayOfWeek switch
    {
        DayOfWeek.Monday => L["Schedule.Monday"],
        DayOfWeek.Tuesday => L["Schedule.Tuesday"],
        DayOfWeek.Wednesday => L["Schedule.Wednesday"],
        DayOfWeek.Thursday => L["Schedule.Thursday"],
        DayOfWeek.Friday => L["Schedule.Friday"],
        DayOfWeek.Saturday => L["Schedule.Saturday"],
        DayOfWeek.Sunday => L["Schedule.Sunday"],
        _ => day.ToString("ddd")
    };

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
