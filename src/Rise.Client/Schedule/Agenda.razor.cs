using System.Timers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Schedule;
using Rise.Shared.Common;

namespace Rise.Client.Schedule
{
    public partial class Agenda : ComponentBase, IAsyncDisposable
    {
        [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;
        [Parameter] public EventCallback<DateTime> OnDateChanged { get; set; }
        private ScheduleDto.Schedule? SelectedSchedule;
        private List<ScheduleDto.Schedule>? schedule;
        private DotNetObjectReference<Agenda>? dotNetRef;
        private string swipeClass = string.Empty;
        private System.Timers.Timer? currentTimeTimer;
        private DateTime currentTime = DateTime.Now;

        [Inject] public required IScheduleService ScheduleService { get; set; }
        [Inject] public required IJSRuntime JSRuntime { get; set; }

        private string GetHeaderTitle() => SelectedDate.Date == DateTime.Today
            ? L["Schedule.Today"]
            : SelectedDate.ToString("dddd, d MMM", System.Globalization.CultureInfo.CurrentCulture);

        protected override async Task OnParametersSetAsync()
        {
            await LoadSchedulesAsync();
        }

        private async Task LoadSchedulesAsync()
        {
            var start = SelectedDate.Date;
            var end = SelectedDate.Date.AddDays(1).AddTicks(-1);

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
                await JSRuntime.InvokeVoidAsync("initSwipe", "dayViewContainer", dotNetRef);

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
            StateHasChanged();
        }

        public List<ScheduleDto.Schedule> DaySchedules =>
            schedule?.Where(r => r.StartDateTime.Date == SelectedDate.Date).ToList()
            ?? new List<ScheduleDto.Schedule>();

        public void GoToToday()
        {
            SelectedDate = DateTime.Today;
            StateHasChanged();
        }

        private async Task AnimateSwipe(string direction)
        {
            swipeClass = direction == "left" ? "swipe-left" : "swipe-right";
            StateHasChanged();

            await Task.Delay(300);

            swipeClass = string.Empty;
            StateHasChanged();
        }

        public async Task PreviousDayAnimated()
        {
            await AnimateSwipe("right");
            PreviousDay();
        }

        public async Task NextDayAnimated()
        {
            await AnimateSwipe("left");
            NextDay();
        }

        public async Task PreviousDay()
        {
            var newDate = ScheduleHelpers.PreviousWeekday(SelectedDate);
            await OnDateChanged.InvokeAsync(newDate);
        }

        public async Task NextDay()
        {
            var newDate = ScheduleHelpers.NextWeekday(SelectedDate);
            await OnDateChanged.InvokeAsync(newDate);
        }

        public void OpenDetails(ScheduleDto.Schedule schedule)
        {
            SelectedSchedule = schedule;
            StateHasChanged();
        }

        public void CloseDetails()
        {
            SelectedSchedule = null;
            StateHasChanged();
        }

        private bool IsToday() => SelectedDate.Date == DateTime.Today;

        [JSInvokable]
        public async Task SwipeNext()
        {
            await NextDayAnimated();
        }

        [JSInvokable]
        public async Task SwipePrevious()
        {
            await PreviousDayAnimated();
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
}
