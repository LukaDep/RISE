namespace Rise.Client.Home.Widgets;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.News;

/// <summary>
/// Dashboard widget that displays the latest news article.
/// Shows a preview with navigation to full news section.
/// </summary>
public partial class NewsWidget : ComponentBase
{
    private NewsDto.Index? News { get; set; }
    private bool _loading;
    private string? _error;
    
    /// <summary>Callback when widget is removed.</summary>
    [Parameter] public EventCallback<Guid> OnRemove { get; set; }
    
    /// <summary>Indicates if edit mode is active.</summary>
    [Parameter] public bool EditMode { get; set; }
    
    /// <summary>Widget index in the grid.</summary>
    [Parameter] public int Index { get; set; }
    
    /// <summary>Unique widget identifier.</summary>
    [Parameter] public Guid WidgetId { get; set; }
    
    /// <summary>JavaScript runtime for interop.</summary>
    [Inject] public IJSRuntime Js { get; set; } = default!;
    
    /// <summary>Navigation manager for routing.</summary>
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;
    
    /// <summary>Service for news data.</summary>
    [Inject] public INewsService NewsClientService { get; set; } = default!;
    
    /// <summary>
    /// Loads the latest news article on initialization.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        try
        {
            var result = await NewsClientService.GetIndexAsync(new QueryRequest.DateRange()
            {
                Skip = 0,
                Take = 1
            });
            News = result.Value.News.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _loading = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Navigates to the news overview page.
    /// </summary>
    private void More()
    {

        NavigationManager.NavigateTo("/news");
    }
    
    /// <summary>
    /// Navigates to the full article detail page for the current news item.
    /// </summary>
    private void ReadMore()
    {
        if (News != null)
        {
            NavigationManager.NavigateTo($"/news/{News.Id}");
        }
    }
}