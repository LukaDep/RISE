using Microsoft.AspNetCore.Components;

namespace Rise.Client.Schedule;

/// <summary>
/// Reusable navigation header component with previous/next buttons and a title
/// </summary>
public partial class NavigationHeader : ComponentBase
{
    [Parameter, EditorRequired] public string Title { get; set; } = string.Empty;
    [Parameter] public string PreviousText { get; set; } = "Previous";
    [Parameter] public string NextText { get; set; } = "Next";
    [Parameter] public EventCallback OnPrevious { get; set; }
    [Parameter] public EventCallback OnNext { get; set; }
}
