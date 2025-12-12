using Microsoft.AspNetCore.Components;

namespace Rise.Client.Schedule;

/// <summary>
/// Reusable navigation header component with previous/next buttons and a title.
/// </summary>
public partial class NavigationHeader : ComponentBase
{
    /// <summary>The title text to display in the header.</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = string.Empty;
    
    /// <summary>Text for the previous button.</summary>
    [Parameter] public string PreviousText { get; set; } = "Previous";
    
    /// <summary>Text for the next button.</summary>
    [Parameter] public string NextText { get; set; } = "Next";
    
    /// <summary>Callback when previous button is clicked.</summary>
    [Parameter] public EventCallback OnPrevious { get; set; }
    
    /// <summary>Callback when next button is clicked.</summary>
    [Parameter] public EventCallback OnNext { get; set; }
}
