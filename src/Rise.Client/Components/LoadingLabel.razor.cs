using Microsoft.AspNetCore.Components;

namespace Rise.Client.Components;

public partial class LoadingLabel : ComponentBase
{
    [Parameter, EditorRequired] public string? Item { get; set; }
}
