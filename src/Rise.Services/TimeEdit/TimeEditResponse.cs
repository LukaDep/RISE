using System.Collections.Generic;

// Deze klasse weerspiegelt de JSON-structuur van TimeEdit (zoals je eerder toonde)
// en dient enkel als DTO — geen domeinlogica!
namespace Rise.Services.TimeEdit
{
  public class TimeEditResponse
  {
    public List<string> ColumnHeaders { get; set; }
    public TimeEditInfo Info { get; set; }
    public List<TimeEditReservation> Reservations { get; set; }
  }

  public class TimeEditInfo
  {
    public int ReservationLimit { get; set; }
    public int ReservationCount { get; set; }
  }

  public class TimeEditReservation
  {
    public string Id { get; set; }
    public string StartDate { get; set; }   // dd-MM-yyyy
    public string StartTime { get; set; }   // HH:mm
    public string EndDate { get; set; }     // dd-MM-yyyy
    public string EndTime { get; set; }     // HH:mm
    public List<string> Columns { get; set; }
  }
}
