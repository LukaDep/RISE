namespace Rise.Client.Home.Widgets;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.Grades;

/// <summary>
/// Dashboard widget that displays the most recent grade.
/// Shows a preview with navigation to full grades section.
/// </summary>
public partial class GradesWidget : ComponentBase
{
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
    
    /// <summary>Service for grades data.</summary>
    [Inject]
    public IGradesService GradesClientService { get; set; } = default!;
    
    private GradesDto.Grade? Grade { get; set; }
    private bool _loading;
    private string? _error;

    /// <summary>
    /// Loads the most recent grade on initialization.
    /// </summary>
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

    /// <summary>
    /// Navigates to the full grades overview page.
    /// </summary>
    private void More()
    {
        NavigationManager.NavigateTo("/grades");
    }
}