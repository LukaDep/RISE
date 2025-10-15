using Microsoft.AspNetCore.Components;
using Rise.Shared.Schedule;
using Rise.Shared.Common;

namespace Rise.Client.Schedule;

public partial class MonthView
{
  [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;

  // Schedule data from service
  private List<ScheduleDto.Reservation>? schedule;

  [Inject] public required IScheduleService ScheduleService { get; set; }

  private string[] DaysOfWeek = { "Ma", "Di", "Wo", "Do", "Vr", "Za", "Zo" };

  protected override async Task OnInitializedAsync()
  {
    var request = new QueryRequest.SkipTake
    {
      Skip = 0,
      Take = 200, // More items for month view
      OrderBy = "Id",
    };

    var result = await ScheduleService.GetIndexAsync(request);
    schedule = result.Value?.Reservations;
  }

  private DateTime FirstDayOfMonth => new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
  private DateTime LastDayOfMonth => FirstDayOfMonth.AddMonths(1).AddDays(-1);

  private List<DateTime> MonthDays
  {
    get
    {
      var firstDay = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
      var daysFromPrevMonth = ((int)firstDay.DayOfWeek + 6) % 7; // Monday = 0
      var startDate = firstDay.AddDays(-daysFromPrevMonth);

      return Enumerable.Range(0, 42) // 6 weeks * 7 days
                       .Select(i => startDate.AddDays(i))
                       .ToList();
    }
  }

  private List<ScheduleDto.Reservation> GetReservationsForDate(DateTime date) =>
      schedule?
          .Where(r => r.StartDateTime.Date == date.Date)
          .ToList()
      ?? new List<ScheduleDto.Reservation>();

  private bool HasEventsOnDay(DateTime day)
  {
    return schedule?.Any(r => r.StartDateTime.Date == day.Date) ?? false;
  }

  private void PreviousMonth()
  {
    SelectedDate = SelectedDate.AddMonths(-1);
  }

  private void NextMonth()
  {
    SelectedDate = SelectedDate.AddMonths(1);
  }

  private bool IsToday(DateTime date) => date.Date == DateTime.Today;

  private bool IsWeekend(DateTime date) => 
    date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

  private string GetEventTypeColor(string type) => type.ToLower() switch
  {
    "activerend hoorcollege" => "border-blue-500",
    "practicum" => "border-green-500",
    "seminarie" => "border-orange-500",
    _ => "border-gray-500"
  };

  private string GetEventTypeBgColor(string type) => type.ToLower() switch
  {
    "activerend hoorcollege" => "bg-blue-100 text-blue-800",
    "practicum" => "bg-green-100 text-green-800",
    "seminarie" => "bg-orange-100 text-orange-800",
    _ => "bg-gray-100 text-gray-800"
  };
}