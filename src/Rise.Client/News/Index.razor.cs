using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Client.News;

public partial class Index
{
    private bool isFilterOpen = false;
    private void ToggleFilter() => isFilterOpen = !isFilterOpen;

    private IEnumerable<NewsDto.Index>? news;
    private IEnumerable<NewsDto.Index>? filteredNews;
    
    [Inject] public required INewsService NewsService { get; set; }

    //dit id voor server-side filtering, als da er nog zou komen
    // [SupplyParameterFromQuery]
    // public string? SearchTerm { get; set; }

    private string? searchTerm;

    protected override async Task OnInitializedAsync()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 50,
            OrderBy = "Id",
            //SearchTerm = SearchTerm
        };

        var result = await NewsService.GetIndexAsync(request);
        news = result.Value.News;
        filteredNews = news;
    }

    //ff client-side filtering, server-side kan later komen
    private void SearchTermChanged(ChangeEventArgs e)
    {
        searchTerm = e.Value?.ToString();
        filteredNews = !string.IsNullOrWhiteSpace(searchTerm)
            ? news?.Where(n => n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            : news;
    }
}