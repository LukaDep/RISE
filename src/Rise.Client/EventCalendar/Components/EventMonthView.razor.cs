using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Client.EventCalendar.Components
{
    /// <summary>
    /// Monthly calendar view component for events.
    /// Displays events in a calendar grid with swipe navigation.
    /// </summary>
    public partial class EventMonthView : IAsyncDisposable
    {
        /// <summary>Currently selected date.</summary>
        [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;

        /// <summary>Callback when a day is clicked.</summary>
        [Parameter] public EventCallback<DateTime> OnDayClick { get; set; }

        /// <summary>List of events to display.</summary>
        [Parameter] public List<EventDTO.Index>? Events { get; set; }

        /// <summary>Service for event data.</summary>
        [Inject] public required IEventService EventService { get; set; }
        
        /// <summary>JavaScript runtime for interop.</summary>
        [Inject] public required IJSRuntime JSRuntime { get; set; }

        private DotNetObjectReference<EventMonthView>? dotNetRef;
        private string swipeClass = string.Empty;

        private List<DateTime> WeekDays => GetWeekDays(SelectedDate);

        /// <summary>
        /// Calculates the 7 days of the week containing the specified date.
        /// Week starts on Monday (ISO 8601 standard).
        /// </summary>
        /// <param name="date">Any date within the desired week.</param>
        /// <returns>List of 7 DateTime objects representing Monday through Sunday.</returns>
        private List<DateTime> GetWeekDays(DateTime date)
        {
            var firstDayOfWeek = date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
            return Enumerable.Range(0, 7).Select(i => firstDayOfWeek.AddDays(i)).ToList();
        }

        /// <summary>
        /// Gets the abbreviated day name (e.g., MON, TUE) for a given date.
        /// Uses the current culture's formatting for localization.
        /// </summary>
        /// <param name="day">The date to get the abbreviation for.</param>
        /// <returns>Uppercase 3-letter day abbreviation.</returns>
        private string GetDayAbbreviation(DateTime day)
        {
            return day.ToString("ddd", System.Globalization.CultureInfo.CurrentCulture).ToUpper();
        }

        protected override async Task OnInitializedAsync()
        {
        }

        /// <summary>
        /// Sets up swipe gesture handling after the first render.
        /// Creates a .NET object reference for JavaScript callbacks and initializes
        /// the swipe detection on the month view container element.
        /// </summary>
        /// <param name="firstRender">True if this is the first render.</param>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetRef = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("initSwipe", "monthViewContainer", dotNetRef);
            }
        }

        /// <summary>
        /// Triggers a CSS swipe animation in the specified direction.
        /// Applies the animation class, waits for CSS transition (250ms),
        /// then removes the class. Used for visual feedback during navigation.
        /// </summary>
        /// <param name="direction">Animation direction: "left" for next, "right" for previous.</param>
        private async Task AnimateSwipe(string direction)
        {
            swipeClass = direction == "left" ? "swipe-left" : "swipe-right";
            StateHasChanged();
            await Task.Delay(250);
            swipeClass = string.Empty;
            StateHasChanged();
        }

        /// <summary>
        /// Navigates the calendar to today's date.
        /// Sets the selected date to today and invokes the OnDayClick callback
        /// to notify parent components of the change.
        /// </summary>
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

        /// <summary>
        /// Gets all days to display in the calendar grid (42 days = 6 weeks × 7 days).
        /// Includes padding days from previous and next months to fill the grid.
        /// Starts from the Monday of the week containing the first day of the month.
        /// </summary>
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

        /// <summary>
        /// Gets all events scheduled for a specific date.
        /// Filters the events list by matching the date portion of StartDateTime.
        /// </summary>
        /// <param name="date">The date to find events for.</param>
        /// <returns>List of events on that date, or empty list if none.</returns>
        private List<EventDTO.Index> GetSchedulesForDate(DateTime date) =>
            Events?.Where(r => r.StartDateTime.Date == date.Date).ToList() ?? new List<EventDTO.Index>();

        /// <summary>
        /// Checks if there are any events on a specific day.
        /// Used to show event indicators on calendar day cells.
        /// </summary>
        /// <param name="day">The day to check.</param>
        /// <returns>True if at least one event exists on that day.</returns>
        private bool HasEventsOnDay(DateTime day) =>
            Events?.Any(r => r.StartDateTime.Date == day.Date) ?? false;

        /// <summary>
        /// Gets a distinct list of event types occurring in the current month.
        /// Used to display filter chips or legend for the calendar.
        /// Types are sorted alphabetically.
        /// </summary>
        /// <returns>Sorted list of unique event type strings in the current month.</returns>
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

        /// <summary>
        /// Navigates to the previous month with a slide-right animation.
        /// </summary>
        public async Task PreviousMonthAnimated()
        {
            await AnimateSwipe("right");
            PreviousMonth();
            StateHasChanged();
        }

        /// <summary>
        /// Navigates to the next month with a slide-left animation.
        /// </summary>
        public async Task NextMonthAnimated()
        {
            await AnimateSwipe("left");
            NextMonth();
            StateHasChanged();
        }

        /// <summary>Decrements the selected date by one month.</summary>
        private void PreviousMonth() => SelectedDate = SelectedDate.AddMonths(-1);
        
        /// <summary>Increments the selected date by one month.</summary>
        private void NextMonth() => SelectedDate = SelectedDate.AddMonths(1);

        /// <summary>
        /// JavaScript-invokable method called when user swipes left.
        /// Navigates to the next month with animation.
        /// </summary>
        [JSInvokable]
        public async Task SwipeNext()
        {
            await NextMonthAnimated();
        }

        /// <summary>
        /// JavaScript-invokable method called when user swipes right.
        /// Navigates to the previous month with animation.
        /// </summary>
        [JSInvokable]
        public async Task SwipePrevious()
        {
            await PreviousMonthAnimated();
        }

        /// <summary>
        /// Navigates to the day view for a specific date.
        /// Invokes the OnDayClick callback to notify parent components.
        /// </summary>
        /// <param name="date">The date to view in detail.</param>
        private async Task GoToDayView(DateTime date)
        {
            if (OnDayClick.HasDelegate)
            {
                await OnDayClick.InvokeAsync(date);
            }
        }
        /// <summary>
        /// Gets the CSS background color classes for an event type badge.
        /// Maps event types to HOGENT brand colors for consistent styling.
        /// </summary>
        /// <param name="type">The event type (e.g., "welzijn", "andere").</param>
        /// <returns>CSS classes for background and text color.</returns>
        public static string GetEventTypeBgColor(string type) => type.ToLower() switch
        {
            "welzijn" => "bg-hogent-education-30 text-hogent-education",
            "andere" => "bg-hogent-it-30 text-hogent-it",
            _ => "bg-hogent-black-30 text-hogent-black"
        };

        /// <summary>
        /// Disposes the .NET object reference used for JavaScript interop.
        /// Called when the component is disposed to prevent memory leaks.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            dotNetRef?.Dispose();
        }
    }
}
