using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Client.EventCalendar.Components
{
    public partial class EventMonthView : IAsyncDisposable
    {
        [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;

        [Parameter] public EventCallback<DateTime> OnDayClick { get; set; }

        [Parameter] public List<EventDTO.Index>? Events { get; set; }

        [Inject] public required IEventService EventService { get; set; }
        [Inject] public required IJSRuntime JSRuntime { get; set; }

        private DotNetObjectReference<EventMonthView>? dotNetRef;
        private string swipeClass = string.Empty;

        private List<DateTime> WeekDays => GetWeekDays(SelectedDate);

        private List<DateTime> GetWeekDays(DateTime date)
        {
            var firstDayOfWeek = date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
            return Enumerable.Range(0, 7).Select(i => firstDayOfWeek.AddDays(i)).ToList();
        }

        private string GetDayAbbreviation(DateTime day)
        {
            return day.ToString("ddd", System.Globalization.CultureInfo.CurrentCulture).ToUpper();
        }

        protected override async Task OnInitializedAsync()
        {
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

        private List<EventDTO.Index> GetSchedulesForDate(DateTime date) =>
            Events?.Where(r => r.StartDateTime.Date == date.Date).ToList() ?? new List<EventDTO.Index>();

        private bool HasEventsOnDay(DateTime day) =>
            Events?.Any(r => r.StartDateTime.Date == day.Date) ?? false;

        private List<string> GetTypesInMonth()
        {
            var currentMonthStart = FirstDayOfMonth;
            var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

            var types = Events?
                .Where(r => r.StartDateTime.Date >= currentMonthStart && r.StartDateTime.Date <= currentMonthEnd)
                .Select(r => r.Type)
                .Distinct()
                .OrderBy(w => w)
                .ToList() ?? new List<string>();

            return types;
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
        public static string GetEventTypeBgColor(string type) => type.ToLower() switch
        {
            "welzijn" => "bg-hogent-education-30 text-hogent-education",
            "andere" => "bg-hogent-it-30 text-hogent-it",
            _ => "bg-hogent-black-30 text-hogent-black"
        };

        public async ValueTask DisposeAsync()
        {
            dotNetRef?.Dispose();
        }
    }
}
