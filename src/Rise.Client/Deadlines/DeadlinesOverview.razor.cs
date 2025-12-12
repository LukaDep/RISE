using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Deadlines;

namespace Rise.Client.Deadlines
{
    /// <summary>
    /// Code-behind for the DeadlinesOverview page component.
    /// Displays upcoming and expired deadlines with date range filtering.
    /// </summary>
    public partial class DeadlinesOverview
    {
        /// <summary>Service for deadline data operations.</summary>
        [Inject] public required IDeadlineService DeadlinesService { get; set; }

        private int skip = 0;
        private int take = 100;
        
        /// <summary>Search term for filtering deadlines.</summary>
        public string? SearchTerm { get; set; }
        
        /// <summary>Start date filter.</summary>
        public DateTime? StartDate { get; set; }
        
        /// <summary>End date filter.</summary>
        public DateTime? EndDate { get; set; }

        private IEnumerable<DeadlineDto.Index>? Deadlines { get; set; }

        /// <summary>
        /// Deadlines that have expired (EndDate is in the past).
        /// </summary>
        private IEnumerable<DeadlineDto.Index> ExpiredDeadlines =>
            Deadlines?.Where(d => d.EndDate < DateTime.Now).OrderByDescending(d => d.EndDate) ?? Enumerable.Empty<DeadlineDto.Index>();

        /// <summary>
        /// Deadlines that are still upcoming (EndDate is in the future or today)
        /// </summary>
        private IEnumerable<DeadlineDto.Index> UpcomingDeadlines =>
            Deadlines?.Where(d => d.EndDate >= DateTime.Now).OrderBy(d => d.EndDate) ?? Enumerable.Empty<DeadlineDto.Index>();

        /// <summary>
        /// Count of active (non-expired) deadlines
        /// </summary>
        private int ActiveDeadlinesCount => UpcomingDeadlines.Count();

        /// <summary>
        /// Initializes the component and loads deadline data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            var result = await GetData();
            Deadlines = result.Value?.Deadlines;
        }

        /// <summary>
        /// Fetches deadline data from the service with date range filtering.
        /// </summary>
        /// <returns>Result containing the deadline response.</returns>
        protected async Task<Result<DeadlineResponse.Index>> GetData()
        {
            QueryRequest.DateRange request = new()
            {
                Skip = skip,
                Take = take,
                SearchTerm = SearchTerm ?? "",
                StartDate = StartDate,
                EndDate = EndDate
            };

            return await DeadlinesService.GetIndexAsync(request);
        }
    }
}
