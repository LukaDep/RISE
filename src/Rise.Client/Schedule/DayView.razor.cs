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

        public string GetEventTypeBgColor(string type) => type.ToLower() switch
        {
            "hoorcollege" => "bg-hogent-education-15 text-hogent-education",
            "activerend hoorcollege" => "bg-hogent-it-15 text-hogent-it",
            "practicum" => "bg-hogent-green-15 text-hogent-green",
            "werkcollege" => "bg-hogent-orange-15 text-hogent-orange",
            "seminarie" => "bg-hogent-business-15 text-hogent-business",
            _ => "bg-hogent-black-15 text-hogent-black"
        };
    }
}
