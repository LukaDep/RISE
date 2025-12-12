using Microsoft.AspNetCore.Components;

namespace Rise.Client.Components;

/// <summary>
/// Simple loading label component that displays a loading message.
/// Shows what item is currently being loaded.
/// </summary>
public partial class LoadingLabel : ComponentBase
{
    /// <summary>The name of the item being loaded.</summary>
    [Parameter, EditorRequired] public string? Item { get; set; }
}
