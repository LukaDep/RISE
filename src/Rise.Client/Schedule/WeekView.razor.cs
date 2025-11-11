using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.Schedule;
using System.Timers;

namespace Rise.Client.Schedule;

public partial class WeekView : IAsyncDisposable
{
    [Parameter] public EventCallback<DateTime> OnDayClick { get; set; }
    [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;

    private ScheduleDto.Schedule? SelectedSchedule;
    private List<ScheduleDto.Schedule>? schedule;

    [Inject] public required IScheduleService ScheduleService { get; set; }
    [Inject] public required IJSRuntime JSRuntime { get; set; }

    private DotNetObjectReference<WeekView>? dotNetRef;
    private string swipeClass = string.Empty;
    private System.Timers.Timer? currentTimeTimer;
    private DateTime currentTime = DateTime.Now;

    protected override async Task OnInitializedAsync()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 100,
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
        await Task.Delay(250);
        swipeClass = string.Empty;
        StateHasChanged();
    }

    private void OpenDetails(ScheduleDto.Schedule schedule)
    {
        SelectedSchedule = schedule;
        StateHasChanged();
    }

    private void CloseDetails()
    {
        SelectedSchedule = null;
        StateHasChanged();
    }

    private DateTime WeekStartDate
    {
        get
        {
            var dayOfWeek = (int)SelectedDate.DayOfWeek;
            var daysToSubtract = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
            return SelectedDate.AddDays(-daysToSubtract);
        }
    }

    private int WeekNumber => System.Globalization.ISOWeek.GetWeekOfYear(WeekStartDate);

    private List<DateTime> WeekDays =>
        Enumerable.Range(0, 5)
                  .Select(i => WeekStartDate.AddDays(i))
                  .ToList();

    private async Task GoToToday()
    {
        SelectedDate = DateTime.Today;
        if (OnDayClick.HasDelegate)
        {
            await OnDayClick.InvokeAsync(DateTime.Today);
        }
        StateHasChanged();
    }

    public async Task PreviousWeekAnimated()
    {
        await AnimateSwipe("right");
        PreviousWeek();
    }

    public async Task NextWeekAnimated()
    {
        await AnimateSwipe("left");
        NextWeek();
    }

    private void PreviousWeek() => SelectedDate = SelectedDate.AddDays(-7);
    private void NextWeek() => SelectedDate = SelectedDate.AddDays(7);

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

    private List<ScheduleDto.Schedule> GetSchedulesForDate(DateTime date) =>
        schedule?.Where(r => r.StartDateTime.Date == date.Date).ToList()
        ?? new List<ScheduleDto.Schedule>();

    private double GetCurrentTimePosition()
    {
        var now = currentTime;
        var hour = now.Hour;
        var minute = now.Minute;

        if (hour < 8 || hour > 20)
            return -1;

        return ((hour - 8) * 64) + (minute * 64 / 60.0);
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


    private string GetLocalizedDayName(DateTime day)
    {
        return day.DayOfWeek switch
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
    }

    private static string TruncateTitle(string title, int maxLength = 40)
    {
        if (string.IsNullOrEmpty(title))
            return title;

        if (title.Length <= maxLength)
            return title;

        return title.Substring(0, maxLength) + "...";
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
