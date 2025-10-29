using FuzzySharp;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Rise.Shared.Common;
using Rise.Shared.Grades;

namespace Rise.Client.Grades
{
    public partial class Index : ComponentBase
    {
        // input field reference
        protected ElementReference filterInput;
        protected bool isFilterOpen = false;
        protected async Task ToggleFilter()
        {
            isFilterOpen = !isFilterOpen;
            if (isFilterOpen)
            {
                await Task.Yield();
                try
                {
                    await filterInput.FocusAsync();
                }
                catch
                {
                }
            }
        }
        // grades data
        private IEnumerable<GradesDto.Grade> Grades { get; set; } = Array.Empty<GradesDto.Grade>();
        private IEnumerable<GradesDto.Grade> FilteredGrades { get; set; } = Array.Empty<GradesDto.Grade>();
        // injections
        [Inject] public IStringLocalizer<SharedResources> L { get; set; } = default!;
        [Inject] public required IGradesService GradesClientService { get; set; }
        [Inject] public required NavigationManager Navigation { get; set; }
        // filter parameters
        [Parameter, SupplyParameterFromQuery] public string? SearchTerm { get; set; }
        [Parameter, SupplyParameterFromQuery] public string? Year { get; set; }
        [Parameter, SupplyParameterFromQuery] public int? Semester { get; set; }
        // private fields for filters
        private string? searchTerm;
        private string? selectedYear;
        private int? selectedSemester;
        // fuzzy search threshold
        private const int FuzzyScoreThreshold = 60;
        // year options for select
        public List<string> YearOptions { get; } = BuildYearOptions();
        // filter items for SimpleSelects
        protected IEnumerable<KeyValuePair<string, string>> YearItems =>
            new[] { new KeyValuePair<string, string>(string.Empty, L["Grades.YearFilterPlaceholder"]) }
            .Concat(YearOptions.Select(y => new KeyValuePair<string, string>(y, y)));
        protected IEnumerable<KeyValuePair<string, string>> SemesterItems =>
            new[] { new KeyValuePair<string, string>(string.Empty, L["Grades.SemesterFilterPlaceholder"]) }
            .Concat(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>("1", L["Grades.SemesterFilterOption1"]),
                new KeyValuePair<string,string>("2", L["Grades.SemesterFilterOption2"]),
            });

        private static List<string> BuildYearOptions()
        {
            var list = new List<string>();
            var now = DateTime.Now;
            var currentStartYear = now.Month >= 9 ? now.Year : now.Year - 1;
            for (int i = 0; i < 6; i++)
            {
                var start = currentStartYear - i;
                var end = start + 1;
                list.Add($"{start}-{end}");
            }

            return list;
        }

        protected override async Task OnParametersSetAsync()
        {
            QueryRequest.SkipTake request = new()
            {
                Skip = 0,
                Take = 20,
                SearchTerm = SearchTerm,
            };

            var result = await GradesClientService.GetIndexAsync(request);
            Grades = result.Value.Grades;
            FilteredGrades = Grades;
        }
        protected override void OnParametersSet()
        {
            searchTerm = SearchTerm;
        }

        private Task SearchTermChanged(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;
            FilterGrades();
            FilteredGrades = Grades;
            return Task.CompletedTask;
        }
        private void FilterGrades()
        {
            Dictionary<string, object?> parameters = new();
            parameters.Add(nameof(searchTerm), searchTerm);
            var uri = Navigation.GetUriWithQueryParameters(parameters);
            Navigation.NavigateTo(uri);
        }

        private void ApplyFilters()
        {
            var query = Grades.AsQueryable();

            if (!string.IsNullOrWhiteSpace(selectedYear))
            {
                query = query.Where(g => g.Year == selectedYear);
            }

            if (selectedSemester.HasValue)
            {
                query = query.Where(g => g.Semester == selectedSemester.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                var scored = query
                    .Select(g => new
                    {
                        Grade = g,
                        Score = Math.Max(
                            Fuzz.WeightedRatio(term, (g.Name ?? string.Empty).ToLower()),
                            Fuzz.WeightedRatio(term, (g.CourseName ?? string.Empty).ToLower()))
                    })
                    .Where(x => x.Score >= FuzzyScoreThreshold)
                    .OrderByDescending(x => x.Score)
                    .Select(x => x.Grade)
                    .ToList();

                FilteredGrades = scored;
            }
            else
            {
                FilteredGrades = query
                    .OrderBy(g => (DateTime?)(g.Date) ?? DateTime.MaxValue)
                    .ToList();
            }

            StateHasChanged();
        }

        // Handlers for SimpleSelect
        protected async Task OnYearSelected(string? value)
        {
            selectedYear = value;
            ApplyFilters();
        }

        protected async Task OnSemesterSelected(string? value)
        {
            if (int.TryParse(value, out int sem))
                selectedSemester = sem;
            else
                selectedSemester = null;

            ApplyFilters();
        }
    }
}
