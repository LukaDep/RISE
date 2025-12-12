using System.Timers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Schedule;
using Rise.Shared.Common;

namespace Rise.Client.Schedule
{
    /// <summary>
    /// Code-behind for the Agenda page component.
    /// Displays a day view of scheduled classes with swipe navigation.
    /// </summary>
    public partial class Agenda : ComponentBase, IAsyncDisposable
    {
        /// <summary>The currently selected date.</summary>
        [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;
        
        /// <summary>Event callback when date changes.</summary>
        [Parameter] public EventCallback<DateTime> OnDateChanged { get; set; }
        
        private ScheduleDto.Schedule? SelectedSchedule;
        private List<ScheduleDto.Schedule>? schedule;
        private DotNetObjectReference<Agenda>? dotNetRef;
        private string swipeClass = string.Empty;
        private System.Timers.Timer? currentTimeTimer;
        private DateTime currentTime = DateTime.Now;

        /// <summary>Service for schedule data operations.</summary>
        [Inject] public required IScheduleService ScheduleService { get; set; }
        
        /// <summary>JavaScript runtime for interop calls.</summary>
        [Inject] public required IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets the header title ("Today" or formatted date).
        /// </summary>
        private string GetHeaderTitle() => SelectedDate.Date == DateTime.Today
            ? L["Schedule.Today"]
            : SelectedDate.ToString("dddd, d MMM", System.Globalization.CultureInfo.CurrentCulture);

        /// <summary>
        /// Loads schedule data when parameters change.
        /// </summary>
        protected override async Task OnParametersSetAsync()
        {
            await LoadSchedulesAsync();
        }

        /// <summary>
        /// Loads schedules for the selected date.
        /// </summary>
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
        
        /// <summary>
        /// Initializes swipe gestures and timer after first render.
        /// </summary>
        /// <param name="firstRender">True if this is the first render.</param>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetRef = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("initSwipe", "dayViewContainer", dotNetRef);

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
            StateHasChanged();
        }

        /// <summary>
        /// Gets the schedules for the currently selected date.
        /// </summary>
        public List<ScheduleDto.Schedule> DaySchedules =>
            schedule?.Where(r => r.StartDateTime.Date == SelectedDate.Date).ToList()
            ?? new List<ScheduleDto.Schedule>();

        /// <summary>
        /// Navigates to today's schedule.
        /// </summary>
        public void GoToToday()
        {
            SelectedDate = DateTime.Today;
            StateHasChanged();
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

        /// <summary>
        /// Navigates to previous day with animation.
        /// </summary>
        public async Task PreviousDayAnimated()
        {
            await AnimateSwipe("right");
            PreviousDay();
        }

        /// <summary>
        /// Navigates to next day with animation.
        /// </summary>
        public async Task NextDayAnimated()
        {
            await AnimateSwipe("left");
            NextDay();
        }

        /// <summary>
        /// Navigates to the previous weekday.
        /// </summary>
        public async Task PreviousDay()
        {
            var newDate = ScheduleHelpers.PreviousWeekday(SelectedDate);
            await OnDateChanged.InvokeAsync(newDate);
        }

        /// <summary>
        /// Navigates to the next weekday.
        /// </summary>
        public async Task NextDay()
        {
            var newDate = ScheduleHelpers.NextWeekday(SelectedDate);
            await OnDateChanged.InvokeAsync(newDate);
        }

        /// <summary>
        /// Opens the schedule details modal.
        /// </summary>
        /// <param name="schedule">The schedule to show details for.</param>
        public void OpenDetails(ScheduleDto.Schedule schedule)
        {
            SelectedSchedule = schedule;
            StateHasChanged();
        }

        /// <summary>
        /// Closes the schedule details modal.
        /// </summary>
        public void CloseDetails()
        {
            SelectedSchedule = null;
            StateHasChanged();
        }

        /// <summary>
        /// Checks if the selected date is today.
        /// </summary>
        /// <returns>True if selected date is today.</returns>
        private bool IsToday() => SelectedDate.Date == DateTime.Today;

        /// <summary>
        /// JavaScript invokable method for swipe next gesture.
        /// </summary>
        [JSInvokable]
        public async Task SwipeNext()
        {
            await NextDayAnimated();
        }

        /// <summary>
        /// JavaScript invokable method for swipe previous gesture.
        /// </summary>
        [JSInvokable]
        public async Task SwipePrevious()
        {
            await PreviousDayAnimated();
        }

        /// <summary>
        /// Disposes resources including timer and JS interop reference.
        /// </summary>
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
