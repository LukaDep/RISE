using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components; 
using Rise.Shared.Schedule;           
using Rise.Shared.Common;            

namespace Rise.Client.Schedule
{
    public partial class DayView : ComponentBase
    {
        [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;

        private ScheduleDto.Reservation? SelectedReservation;
        private List<ScheduleDto.Reservation>? schedule;

        [Inject] public required IScheduleService ScheduleService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var request = new QueryRequest.SkipTake
            {
                Skip = 0,
                Take = 50,
                OrderBy = "Id"
            };

            var result = await ScheduleService.GetIndexAsync(request);
            schedule = result.Value?.Reservations;
        }

        public List<ScheduleDto.Reservation> DayReservations =>
            schedule?.Where(r => r.StartDateTime.Date == SelectedDate.Date).ToList()
            ?? new List<ScheduleDto.Reservation>();

        public void PreviousDay()
        {
            do { SelectedDate = SelectedDate.AddDays(-1); }
            while (SelectedDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
        }

        public void NextDay()
        {
            do { SelectedDate = SelectedDate.AddDays(1); }
            while (SelectedDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
        }

public void OpenDetails(ScheduleDto.Reservation reservation)
{
    SelectedReservation = reservation;
    StateHasChanged();
}
   

public void CloseDetails()
{
    SelectedReservation = null;
    StateHasChanged(); // popup verdwijnt direct
   

        public string GetEventTypeColor(string type) => type.ToLower() switch
        {
            "activerend hoorcollege" => "border-blue-500",
            "practicum" => "border-green-500",
            "seminarie" => "border-orange-500",
            _ => "border-hogent-black-30"
        };

        public string GetEventTypeBgColor(string type) => type.ToLower() switch
        {
            "activerend hoorcollege" => "bg-blue-100 text-blue-800",
            "practicum" => "bg-green-100 text-green-800",
            "seminarie" => "bg-orange-100 text-orange-800",
            _ => "bg-hogent-black-15 text-hogent-black"
}
        };
    }
}
