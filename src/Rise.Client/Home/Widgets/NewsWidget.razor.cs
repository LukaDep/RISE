namespace Rise.Client.Home.Widgets;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.News;

public partial class NewsWidget : ComponentBase
{
    private NewsDto.Index? News { get; set; }
    private bool _loading;
    private string? _error;
    [Parameter] public EventCallback<Guid> OnRemove { get; set; }
    [Parameter] public bool EditMode { get; set; }
    [Parameter] public int Index { get; set; }
    [Parameter] public Guid WidgetId { get; set; }
    [Inject] public IJSRuntime Js { get; set; } = default!;
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;
    [Inject] public INewsService NewsClientService { get; set; } = default!;
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

    private void More()
    {

        NavigationManager.NavigateTo("/news");
    }
    private void ReadMore()
    {
        if (News != null)
        {
            NavigationManager.NavigateTo($"/news/{News.Id}");
        }
    }
}