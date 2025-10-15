using Microsoft.AspNetCore.Components;
using Rise.Shared.Schedule;
using Rise.Shared.Common;

namespace Rise.Client.Schedule;

public partial class WeekView
{
  private ScheduleDto.Reservation? SelectedReservation;

private void OpenDetails(ScheduleDto.Reservation reservation)
{
    SelectedReservation = reservation;
    StateHasChanged();
}

private void CloseDetails()
{
    SelectedReservation = null;
    StateHasChanged();
}

  [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;

  // Schedule data from service
  private List<ScheduleDto.Reservation>? schedule;

  [Inject] public required IScheduleService ScheduleService { get; set; }

  protected override async Task OnInitializedAsync()
  {
    var request = new QueryRequest.SkipTake
    {
      Skip = 0,
      Take = 100, // Enough items for week view
      OrderBy = "Id",
    };
 
    var result = await ScheduleService.GetIndexAsync(request);
    schedule = result.Value?.Reservations;
  }

  // Calculate week start (Monday)
  private DateTime WeekStartDate
  {
    get
    {
      var dayOfWeek = (int)SelectedDate.DayOfWeek;
      // If Sunday (0), go back 6 days, otherwise go back to Monday
      var daysToSubtract = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
      return SelectedDate.AddDays(-daysToSubtract);
    }
  }

  private DateTime WeekEndDate => WeekStartDate.AddDays(6);
  private int WeekNumber => System.Globalization.ISOWeek.GetWeekOfYear(WeekStartDate);

  private List<DateTime> WeekDays =>
      Enumerable.Range(0, 5) // Only Mon-Fri
                .Select(i => WeekStartDate.AddDays(i))
                .ToList();

  private void PreviousWeek()
  {
    SelectedDate = SelectedDate.AddDays(-7);
  }

  private void NextWeek()
  {
    SelectedDate = SelectedDate.AddDays(7);
  }

  private bool HasEventAtTime(DateTime day, int hour)
  {
    return schedule?.Any(r =>
    {
      return r.StartDateTime.Date == day.Date &&
             r.StartDateTime.Hour <= hour &&
             r.EndDateTime.Hour > hour;
    }) ?? false;
  }

  private ScheduleDto.Reservation? GetEventAtTime(DateTime day, int hour)
  {
    return schedule?.FirstOrDefault(r =>
    {
      return r.StartDateTime.Date == day.Date &&
             r.StartDateTime.Hour <= hour &&
             r.EndDateTime.Hour > hour;
    });
  }

  private List<ScheduleDto.Reservation> GetReservationsForDate(DateTime date) =>
      schedule?
          .Where(r => r.StartDateTime.Date == date.Date)
          .ToList()
      ?? new List<ScheduleDto.Reservation>();

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