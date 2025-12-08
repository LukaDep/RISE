using Microsoft.AspNetCore.Components;
using Rise.Shared.Deadlines;

namespace Rise.Client.Deadlines.Components
{
    public partial class DeadlineCard
    {
        [Parameter] public required DeadlineDto.Index Deadline { get; set; }

        private int DaysRemaining { get; set; }

        protected override void OnInitialized()
        {
            DaysRemaining = Deadline != null
                ? (int)(Deadline.EndDate - DateTime.Now).TotalDays
                : 0;
        }
    }
}
