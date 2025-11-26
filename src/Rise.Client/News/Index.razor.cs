using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Client.News;

public partial class Index
{
    ElementReference _filterInput;
    private bool _isFilterOpen = false;

    private IEnumerable<NewsDto.Index>? _news;

    [Inject] public required INewsService NewsService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;

    [Parameter, EditorRequired]
    [SupplyParameterFromQuery]
    public string? SearchTerm { get; set; }

    [Parameter, SupplyParameterFromQuery]
    public DateTime? StartDate { get; set; }
    [Parameter, SupplyParameterFromQuery]
    public DateTime? EndDate { get; set; }
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

    protected override void OnParametersSet()
    {
        _searchTerm = string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm?.Trim();
        _startDate = StartDate;
        _endDate = EndDate;

    }

    private void SearchTermChanged(string value)
    {
        _searchTerm = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        FilterNews();
    }

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