
using FuzzySharp;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Grades;
namespace Rise.Client.Grades;



public partial class Index : ComponentBase
{
    protected bool isFilterOpen = false;

    private IEnumerable<GradesDto.Grade> Grades { get; set; } = Array.Empty<GradesDto.Grade>();
    private IEnumerable<GradesDto.Grade> FilteredGrades { get; set; } = Array.Empty<GradesDto.Grade>();
    [Inject] public required IGradesService GradesClientService { get; set; }
    [Inject] public required NavigationManager Navigation { get; set; }
    [Parameter, SupplyParameterFromQuery] public string? SearchTerm { get; set; }
    [Parameter, SupplyParameterFromQuery] public string? Year { get; set; }
    [Parameter, SupplyParameterFromQuery] public int? Semester { get; set; }
    private string? searchTerm;
    private string? selectedYear;
    private int? selectedSemester;
    private const int FuzzyScoreThreshold = 60;
    public List<string> YearOptions { get; } = BuildYearOptions();
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

    private void OnSearchTermChanged(string value)
    {
        searchTerm = value;
        ApplyFilters();
    }

    private void FilterGrades()
    {
        Dictionary<string, object?> parameters = new();
        parameters.Add(nameof(searchTerm), searchTerm);
        var uri = Navigation.GetUriWithQueryParameters(parameters);
        Navigation.NavigateTo(uri);
    }

    private static string NormalizeYear(string? y)
    {
        if (string.IsNullOrWhiteSpace(y)) return string.Empty;
        var trimmed = y.Trim();
        return trimmed.Contains('-') ? trimmed.Split('-')[0].Trim() : trimmed;
    }
    private void ApplyFilters()
    {
        var query = Grades.AsQueryable();

        if (!string.IsNullOrWhiteSpace(selectedYear))
        {

            var selectedStart = NormalizeYear(selectedYear);
            query = query.Where(g => NormalizeYear(g.Year) == selectedStart);
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

    protected void OnYearSelected(string? value)
    {
        selectedYear = value;
        ApplyFilters();
    }

    protected void OnSemesterSelected(string? value)
    {
        if (int.TryParse(value, out int sem))
            selectedSemester = sem;
        else
            selectedSemester = null;

        ApplyFilters();
    }

    protected string GetAverageScore()
    {
        var gradesWithScores = FilteredGrades
            .Where(g => g.Score.HasValue && g.MaxPoints.HasValue && g.MaxPoints > 0)
            .ToList();

        if (!gradesWithScores.Any()) return "-";

        var average = gradesWithScores.Average(g => (g.Score!.Value / g.MaxPoints!.Value) * 100);
        return $"{average:F1}%";
    }

    protected int GetPassedCount()
    {
        return FilteredGrades
            .Count(g => g.Score.HasValue && g.MaxPoints.HasValue && g.MaxPoints > 0 && g.Score >= g.MaxPoints * 0.5);
    }

    protected int GetFailedCount()
    {
        return FilteredGrades
            .Count(g => g.Score.HasValue && g.MaxPoints.HasValue && g.MaxPoints > 0 && g.Score < g.MaxPoints * 0.5);
    }
}

