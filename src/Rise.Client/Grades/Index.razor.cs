
using FuzzySharp;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Grades;
namespace Rise.Client.Grades;

/// <summary>
/// Code-behind for the Grades Index page component.
/// Displays a searchable and filterable list of student grades by course.
/// </summary>
public partial class Index : ComponentBase
{
    /// <summary>Indicates whether the filter panel is open.</summary>
    protected bool isFilterOpen = false;

    private IEnumerable<GradesDto.Grade> Grades { get; set; } = Array.Empty<GradesDto.Grade>();
    private IEnumerable<GradesDto.Grade> FilteredGrades { get; set; } = Array.Empty<GradesDto.Grade>();
    
    /// <summary>Service for grades data operations.</summary>
    [Inject] public required IGradesService GradesClientService { get; set; }
    
    /// <summary>Navigation manager for URL handling.</summary>
    [Inject] public required NavigationManager Navigation { get; set; }
    
    /// <summary>Search term from query string.</summary>
    [Parameter, SupplyParameterFromQuery] public string? SearchTerm { get; set; }
    
    /// <summary>Academic year filter from query string.</summary>
    [Parameter, SupplyParameterFromQuery] public string? Year { get; set; }
    
    /// <summary>Semester filter from query string.</summary>
    [Parameter, SupplyParameterFromQuery] public int? Semester { get; set; }
    
    private string? searchTerm;
    private string? selectedYear;
    private int? selectedSemester;
    private const int FuzzyScoreThreshold = 60;
    
    /// <summary>Available academic year options for filtering.</summary>
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

    /// <summary>
    /// Builds the list of academic year options for the year filter dropdown.
    /// </summary>
    /// <returns>A list of academic year strings (e.g., "2024-2025").</returns>
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

    /// <summary>
    /// Loads grades data when query parameters change.
    /// </summary>
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
    /// <summary>
    /// Initializes local filter state from query parameters.
    /// </summary>
    protected override void OnParametersSet()
    {
        searchTerm = SearchTerm;
    }

    /// <summary>
    /// Handles search term changes and triggers filtering.
    /// </summary>
    /// <param name="value">The new search term value.</param>
    private void OnSearchTermChanged(string value)
    {
        searchTerm = value;
        ApplyFilters();
    }

    /// <summary>
    /// Updates the URL with the current search term query parameter.
    /// </summary>
    private void FilterGrades()
    {
        Dictionary<string, object?> parameters = new();
        parameters.Add(nameof(searchTerm), searchTerm);
        var uri = Navigation.GetUriWithQueryParameters(parameters);
        Navigation.NavigateTo(uri);
    }

    /// <summary>
    /// Normalizes an academic year string by extracting the start year.
    /// </summary>
    /// <param name="y">The academic year string (e.g., "2024-2025" or "2024").</param>
    /// <returns>The start year as a string, or empty if input is null/whitespace.</returns>
    private static string NormalizeYear(string? y)
    {
        if (string.IsNullOrWhiteSpace(y)) return string.Empty;
        var trimmed = y.Trim();
        return trimmed.Contains('-') ? trimmed.Split('-')[0].Trim() : trimmed;
    }
    /// <summary>
    /// Applies year, semester, and search term filters to the grades list.
    /// Uses fuzzy matching for search terms.
    /// </summary>
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

    /// <summary>
    /// Handles year filter selection changes.
    /// </summary>
    /// <param name="value">The selected academic year value.</param>
    protected void OnYearSelected(string? value)
    {
        selectedYear = value;
        ApplyFilters();
    }

    /// <summary>
    /// Handles semester filter selection changes.
    /// </summary>
    /// <param name="value">The selected semester value (1 or 2).</param>
    protected void OnSemesterSelected(string? value)
    {
        if (int.TryParse(value, out int sem))
            selectedSemester = sem;
        else
            selectedSemester = null;

        ApplyFilters();
    }

    /// <summary>
    /// Calculates the average score percentage across all filtered grades.
    /// </summary>
    /// <returns>The average score as a percentage string, or "-" if no valid grades.</returns>
    protected string GetAverageScore()
    {
        var gradesWithScores = FilteredGrades
            .Where(g => g.Score.HasValue && g.MaxPoints.HasValue && g.MaxPoints > 0)
            .ToList();

        if (!gradesWithScores.Any()) return "-";

        var average = gradesWithScores.Average(g => (g.Score!.Value / g.MaxPoints!.Value) * 100);
        return $"{average:F1}%";
    }

    /// <summary>
    /// Counts the number of passed grades (score >= 50% of max points).
    /// </summary>
    /// <returns>The count of passed grades.</returns>
    protected int GetPassedCount()
    {
        return FilteredGrades
            .Count(g => g.Score.HasValue && g.MaxPoints.HasValue && g.MaxPoints > 0 && g.Score >= g.MaxPoints * 0.5);
    }

    /// <summary>
    /// Counts the number of failed grades (score < 50% of max points).
    /// </summary>
    /// <returns>The count of failed grades.</returns>
    protected int GetFailedCount()
    {
        return FilteredGrades
            .Count(g => g.Score.HasValue && g.MaxPoints.HasValue && g.MaxPoints > 0 && g.Score < g.MaxPoints * 0.5);
    }
}

