using Microsoft.AspNetCore.Components;

namespace Rise.Client.Components;

/// <summary>
/// Loading indicator component that displays a spinner with context.
/// Shows what content is being loaded to the user.
/// </summary>
public partial class LoadingComponent : ComponentBase
{
    /// <summary>The name of the item being loaded, displayed to the user.</summary>
    [Parameter, EditorRequired] public string? Item { get; set; }
}