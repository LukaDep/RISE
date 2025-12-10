using Microsoft.AspNetCore.Components;
using Rise.Shared.Deadlines;

namespace Rise.Client.Deadlines.Components
{
    public partial class DeadlineCard
    {
        [Parameter] public required DeadlineDto.Index Deadline { get; set; }

        private int DaysRemaining { get; set; }

        /// <summary>
        /// Indicates if the deadline has expired (EndDate is in the past)
        /// </summary>
        private bool IsExpired { get; set; }

        protected override void OnInitialized()
        {
            var now = DateTime.Now;
            DaysRemaining = Deadline != null
                ? (int)Math.Ceiling((Deadline.EndDate - now).TotalDays)
                : 0;
            IsExpired = Deadline != null && Deadline.EndDate < now;
        }

        /// <summary>
        /// Returns the appropriate CSS classes for the urgency badge based on days remaining
        /// </summary>
        private string GetUrgencyBadgeClasses()
        {
            var baseClasses = "inline-flex items-center gap-1 px-2 py-1 rounded-lg text-xs font-semibold";

            return DaysRemaining switch
            {
                <= 1 => $"{baseClasses} bg-red-100 text-red-700",
                <= 3 => $"{baseClasses} bg-orange-100 text-orange-700",
                _ => $"{baseClasses} bg-yellow-100 text-yellow-700"
            };
        }
    }
}
