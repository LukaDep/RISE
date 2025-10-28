using Microsoft.AspNetCore.Components;
using Rise.Shared.Schedule;
using Rise.Shared.Common;

namespace Rise.Client.Schedule;

public partial class MonthView
{
    [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;

    // Callback to parent when a day is clicked
    [Parameter] public EventCallback<DateTime> OnDayClick { get; set; }

    private List<ScheduleDto.Reservation>? schedule;

    [Inject] public required IScheduleService ScheduleService { get; set; }


    protected override async Task OnInitializedAsync()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 200,
            OrderBy = "Id",
        };

        var result = await ScheduleService.GetIndexAsync(request);
        schedule = result.Value?.Reservations;
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

    private List<ScheduleDto.Reservation> GetReservationsForDate(DateTime date) =>
        schedule?.Where(r => r.StartDateTime.Date == date.Date).ToList() ?? new List<ScheduleDto.Reservation>();

    private bool HasEventsOnDay(DateTime day) => schedule?.Any(r => r.StartDateTime.Date == day.Date) ?? false;

    private void PreviousMonth() => SelectedDate = SelectedDate.AddMonths(-1);
    private void NextMonth() => SelectedDate = SelectedDate.AddMonths(1);

    private string GetEventTypeBgColor(string type) => type.ToLower() switch
    {
        "hoorcollege" => "bg-hogent-education-15 text-hogent-education",
        "activerend hoorcollege" => "bg-hogent-it-15 text-hogent-it",
        "practicum" => "bg-hogent-green-15 text-hogent-green",
        "werkcollege" => "bg-hogent-orange-15 text-hogent-orange",
        "seminarie" => "bg-hogent-business-15 text-hogent-business",
        _ => "bg-hogent-black-15 text-hogent-black"
    };

    private async Task GoToDayView(DateTime date)
    {
        if (OnDayClick.HasDelegate)
        {
            await OnDayClick.InvokeAsync(date);
        }
    }
}
