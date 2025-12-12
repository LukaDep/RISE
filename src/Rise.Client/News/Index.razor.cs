using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Client.News;

/// <summary>
/// Code-behind for the News Index page component.
/// Displays a searchable and filterable list of news articles with infinite scroll.
/// </summary>
public partial class Index
{
    ElementReference _filterInput;
    private bool _isFilterOpen = false;

    private IEnumerable<NewsDto.Index>? _news;

    /// <summary>Service for news data operations.</summary>
    [Inject] public required INewsService NewsService { get; set; }
    
    /// <summary>Navigation manager for URL handling.</summary>
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>Search term from query string.</summary>
    [Parameter, EditorRequired]
    [SupplyParameterFromQuery]
    public string? SearchTerm { get; set; }

    /// <summary>Start date filter from query string.</summary>
    [Parameter, SupplyParameterFromQuery]
    public DateTime? StartDate { get; set; }
    
    /// <summary>End date filter from query string.</summary>
    [Parameter, SupplyParameterFromQuery]
    public DateTime? EndDate { get; set; }
    
    /// <summary>JavaScript runtime for interop calls.</summary>
    [Inject] public required IJSRuntime JSRuntime { get; set; }
    
    private int _skip = 0;
    private int _take = 10;
    private int _totalCount;
    private int _currentCount;

    //js scroll to top 
    private bool _initialized;


    // Date range filter items
    private IEnumerable<KeyValuePair<string, string>> DateRangeItems =>
    [
        new(string.Empty, L["News.Filter.All"]),
        new ("today", L["News.Filter.Today"]),
        new ("week", L["News.Filter.Week"]),
        new ("month", L["News.Filter.Month"])
    ];

    private string? _searchTerm;

    private string? _selectedDateRange = string.Empty;
    private DateTime? _startDate;
    private DateTime? _endDate;

    /// <summary>
    /// Loads news data when query parameters change.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        // copy nullable query params locally
        _startDate = StartDate;
        _endDate = EndDate;

        // pass nullable StartDate/EndDate directly (QueryRequest.DateRange uses DateTime?)
        QueryRequest.DateRange request = new()
        {
            Skip = 0,
            Take = 10,
            SearchTerm = SearchTerm,
            StartDate = _startDate,
            EndDate = _endDate,
        };

        var result = await NewsService.GetIndexAsync(request);
        _news = result.Value.News;
        _totalCount = result.Value.TotalCount;
        _currentCount = _news?.Count() ?? 0;
        _skip = 0;
    }

    /// <summary>
    /// Initializes local filter state from query parameters.
    /// </summary>
    protected override void OnParametersSet()
    {
        _searchTerm = string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm?.Trim();
        _startDate = StartDate;
        _endDate = EndDate;

    }

    /// <summary>
    /// Handles search term changes and triggers filtering.
    /// </summary>
    /// <param name="value">The new search term value.</param>
    private void SearchTermChanged(string value)
    {
        _searchTerm = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        FilterNews();
    }

    /// <summary>
    /// Updates the URL with current filter parameters and navigates.
    /// </summary>
    private void FilterNews()
    {
        var parameters = new Dictionary<string, string?>();
        if (!string.IsNullOrWhiteSpace(_searchTerm))
            parameters.Add(nameof(SearchTerm), _searchTerm);

        if (_startDate.HasValue)
            parameters.Add(nameof(StartDate), _startDate.Value.ToString("O"));
        if (_endDate.HasValue)
            parameters.Add(nameof(EndDate), _endDate.Value.ToString("O"));

        var baseUri = NavigationManager.Uri.Split('?', '#')[0];

        string uri;
        if (parameters.Count == 0)
        {
            _startDate = null;
            _endDate = null;
            _selectedDateRange = string.Empty;
            uri = baseUri;
        }
        else
        {
            uri = QueryHelpers.AddQueryString(baseUri, parameters);
        }

        NavigationManager.NavigateTo(uri);
    }

    /// <summary>
    /// Loads additional news articles for infinite scroll pagination.
    /// </summary>
    private async Task LoadMoreNews()
    {
        _skip += _take;
        QueryRequest.DateRange request = new()
        {
            Skip = _skip,
            Take = _take,
            SearchTerm = SearchTerm,
            StartDate = _startDate,
            EndDate = _endDate,
        };

        var result = await NewsService.GetIndexAsync(request);

        _news = _news?.Concat(result.Value.News) ?? result.Value.News;
        _currentCount = _news?.Count() ?? 0;

        StateHasChanged();
    }

    /// <summary>
    /// Initializes scroll-to-top functionality after first render.
    /// </summary>
    /// <param name="firstRender">Whether this is the first render.</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_initialized)
        {
            _initialized = true;
            try
            {
                // Call the global wrapper defined in wwwroot/scrollTop.js
                await JSRuntime.InvokeVoidAsync("initScrollTop", "scrollToTopBtn");
            }
            catch
            {
                // swallow JS errors — avoids breaking rendering if script not present
            }
        }
    }

    /// <summary>
    /// Handles date range filter selection changes.
    /// </summary>
    /// <param name="value">The selected date range option (today, week, month, or empty).</param>
    private void OnDateRangeChanged(string? value)
    {
        _selectedDateRange = value;
        switch (value)
        {
            case "today":
                _startDate = DateTime.Today;
                _endDate = DateTime.Today;
                break;
            case "week":
                int diff = (DateTime.Today.DayOfWeek - DayOfWeek.Monday + 7) % 7;
                var monday = DateTime.Today.AddDays(-diff);
                _startDate = monday;
                _endDate = monday.AddDays(6);
                break;
            case "month":
                var first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var last = first.AddMonths(1).AddDays(-1);
                _startDate = first;
                _endDate = last;
                break;
            default:
                _startDate = null;
                _endDate = null;
                break;
        }
        FilterNews();
    }
}