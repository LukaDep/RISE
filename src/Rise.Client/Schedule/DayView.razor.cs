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
        
        public string GetEventTypeBorderColor(string type) => type.ToLower() switch
        {
            "hoorcollege" => "border-hogent-education-30 text-hogent-education",
            "activerend hoorcollege" => "border-hogent-it-30 text-hogent-it",
            "practicum" => "border-hogent-green-30 text-hogent-green",
            "werkcollege" => "border-hogent-orange-30 text-hogent-orange",
            "seminarie" => "border-hogent-business-30 text-hogent-business",
            _ => "border-hogent-black-30 text-hogent-black"
        };
    }
}
