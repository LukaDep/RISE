using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Deadlines;

namespace Rise.Client.Deadlines
{
    public partial class DeadlinesOverview
    {

        [Inject] public required IDeadlineService DeadlinesService { get; set; }

        private int skip = 0;
        private int take = 100;
        public string? SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        private IEnumerable<DeadlineDto.Index> Deadlines { get; set; }


        protected override async Task OnInitializedAsync()
        {
            var result = await GetData();
            Deadlines = result.Value?.Deadlines;
        }

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
