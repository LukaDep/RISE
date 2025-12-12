using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Rise.Client.Home.Widgets;

/// <summary>
/// Dashboard widget that displays quick links to external resources.
/// Provides shortcuts to commonly used services and websites.
/// </summary>
public partial class LinksWidget : ComponentBase
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
}
