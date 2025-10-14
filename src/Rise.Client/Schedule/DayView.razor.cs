using Microsoft.AspNetCore.Components;
using Rise.Shared.Schedule;
using Rise.Shared.Common;

namespace Rise.Client.Schedule;

public partial class DayView
{
  [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;

  // Schedule data from service
  private List<ScheduleDto.Reservation>? schedule;

  [Inject] public required IScheduleService ScheduleService { get; set; }

  protected override async Task OnInitializedAsync()
  {
    var request = new QueryRequest.SkipTake
    {
      Skip = 0,
      Take = 50,
      OrderBy = "Id",
    };

    var result = await ScheduleService.GetIndexAsync(request);
    schedule = result.Value?.Reservations;
  }

  private List<ScheduleDto.Reservation> DayReservations =>
      schedule?
          .Where(r => r.StartDateTime.Date == SelectedDate.Date)
          .ToList()
      ?? new List<ScheduleDto.Reservation>();

  private void PreviousDay()
  {
    // Skip weekends
    do
    {
      SelectedDate = SelectedDate.AddDays(-1);
    } while (SelectedDate.DayOfWeek == DayOfWeek.Saturday || SelectedDate.DayOfWeek == DayOfWeek.Sunday);
  }

  private void NextDay()
  {
    // Skip weekends
    do
    {
      SelectedDate = SelectedDate.AddDays(1);
    } while (SelectedDate.DayOfWeek == DayOfWeek.Saturday || SelectedDate.DayOfWeek == DayOfWeek.Sunday);
  }

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