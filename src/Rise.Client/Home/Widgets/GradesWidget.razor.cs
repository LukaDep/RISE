using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.Grades;

namespace Rise.Client.Home.Widgets;

public partial class GradesWidget : ComponentBase
{
    [Parameter] public EventCallback<Guid> OnRemove { get; set; }
    [Parameter] public bool EditMode { get; set; }
    [Parameter] public int Index { get; set; }
    [Parameter] public Guid WidgetId { get; set; }
    [Inject] public IJSRuntime Js { get; set; } = default!;
    private GradesDto.Grade? Grade { get; set; }
    private bool _loading;
    private string? _error;

    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        try
        {
            var result = await GradesClientService.GetIndexAsync(new QueryRequest.SkipTake
            {
                Skip = 0,
                Take = 1,
                OrderBy = "Date desc"
            });
            Grade = result.Value?.Grades.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _error = "Failed to load grades.";
            Console.Error.WriteLine(ex);
        }
        finally
        {
            _loading = false;
            StateHasChanged();
        }
    }

    private void More()
    {
        NavigationManager.NavigateTo("/grades");
    }
}