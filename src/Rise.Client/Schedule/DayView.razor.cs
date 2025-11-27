using System.Timers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Schedule;
using Rise.Shared.Common;

namespace Rise.Client.Schedule
{
    public partial class DayView : ComponentBase, IAsyncDisposable
    {
        [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;
        [Parameter] public DateTime? StartDate { get; set; }
        [Parameter] public DateTime? EndDate { get; set; }
        [Parameter] public EventCallback<DateTime> OnDateChanged { get; set; }
        private ScheduleDto.Schedule? SelectedSchedule;
        private List<ScheduleDto.Schedule>? schedule;
        private DotNetObjectReference<DayView>? dotNetRef;

        private string swipeClass = string.Empty;
        private System.Timers.Timer? currentTimeTimer;
        private DateTime currentTime = DateTime.Now;

        [Inject] public required IScheduleService ScheduleService { get; set; }
        [Inject] public required IJSRuntime JSRuntime { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await LoadSchedulesAsync();
        }

        private async Task LoadSchedulesAsync()
        {
            var start = StartDate ?? SelectedDate.Date;
            var end = EndDate ?? SelectedDate.Date.AddDays(1).AddTicks(-1);

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

            await Task.Delay(250);

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
            var newDate = SelectedDate;
            do { newDate = newDate.AddDays(-1); }
            while (newDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
            await OnDateChanged.InvokeAsync(newDate);
        }

        public async Task NextDay()
        {
            var newDate = SelectedDate;
            do { newDate = newDate.AddDays(1); }
            while (newDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
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

        private double GetCurrentTimePosition()
        {
            var now = currentTime;
            var hour = now.Hour;
            var minute = now.Minute;

            if (hour < 8 || hour > 20)
                return -1;

            return ((hour - 8) * 64) + (minute * 64 / 60.0);
        }

        private bool IsToday()
        {
            return SelectedDate.Date == DateTime.Today;
        }

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

        public static string TruncateTitle(string title, int maxLength = 40)
        {
            if (string.IsNullOrEmpty(title))
                return title;

            if (title.Length <= maxLength)
                return title;

            return title.Substring(0, maxLength) + "...";
        }

        public static string GetEventTypeBgColor(string type) => type.ToLower() switch
        {
            "hoorcollege" => "bg-hogent-education-30 text-hogent-education",
            "activerend hoorcollege" => "bg-hogent-it-30 text-hogent-it",
            "practicum" => "bg-hogent-green-30 text-hogent-green",
            "werkcollege" => "bg-hogent-orange-30 text-hogent-orange",
            "seminarie" => "bg-hogent-business-30 text-hogent-business",
            _ => "bg-hogent-black-30 text-hogent-black"
        };


        public static string GetEventTypeBorderColor(string type) => type.ToLower() switch
        {
            "hoorcollege" => "border-hogent-education-30 text-hogent-education",
            "activerend hoorcollege" => "border-hogent-it-30 text-hogent-it",
            "practicum" => "border-hogent-green-30 text-hogent-green",
            "werkcollege" => "border-hogent-orange-30 text-hogent-orange",
            "seminarie" => "border-hogent-business-30 text-hogent-business",
            _ => "border-hogent-black-30 text-hogent-black"
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
}
