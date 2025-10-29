using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Schedule;
using Rise.Shared.Common;

namespace Rise.Client.Schedule
{
    public partial class DayView : ComponentBase, IAsyncDisposable
    {
        [Parameter] public DateTime SelectedDate { get; set; } = DateTime.Today;

        private ScheduleDto.Reservation? SelectedReservation;
        private List<ScheduleDto.Reservation>? schedule;
        private DotNetObjectReference<DayView>? dotNetRef;

        private string swipeClass = string.Empty;

        [Inject] public required IScheduleService ScheduleService { get; set; }
        [Inject] public required IJSRuntime JSRuntime { get; set; }

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetRef = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("initSwipe", "dayViewContainer", dotNetRef);
            }
        }

        public List<ScheduleDto.Reservation> DayReservations =>
            schedule?.Where(r => r.StartDateTime.Date == SelectedDate.Date).ToList()
            ?? new List<ScheduleDto.Reservation>();

        public void GoToToday()
        {
            SelectedDate = DateTime.Today;
            StateHasChanged();
        }

        private async Task AnimateSwipe(string direction)
        {
            swipeClass = direction == "left" ? "swipe-left" : "swipe-right";
            StateHasChanged();

            // Wait for the CSS animation to finish
            await Task.Delay(250);

            swipeClass = string.Empty;
            StateHasChanged();
        }

        public async Task PreviousDayAnimated()
        {
            await AnimateSwipe("right");
            PreviousDay();
        }

        public async Task NextDayAnimated()
        {
            await AnimateSwipe("left");
            NextDay();
        }

        public void PreviousDay()
        {
            do { SelectedDate = SelectedDate.AddDays(-1); }
            while (SelectedDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
            StateHasChanged();
        }

        public void NextDay()
        {
            do { SelectedDate = SelectedDate.AddDays(1); }
            while (SelectedDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
            StateHasChanged();
        }

        public void OpenDetails(ScheduleDto.Reservation reservation)
        {
            SelectedReservation = reservation;
            StateHasChanged();
        }

        public void CloseDetails()
        {
            SelectedReservation = null;
            StateHasChanged();
        }

        [JSInvokable]
        public async Task SwipeNext()
        {
            await NextDayAnimated();
        }

        [JSInvokable]
        public async Task SwipePrevious()
        {
            await PreviousDayAnimated();
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

        public async ValueTask DisposeAsync()
        {
            dotNetRef?.Dispose();
        }
    }
}
