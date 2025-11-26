using Microsoft.AspNetCore.Components;

namespace Rise.Client.Components;
public partial class LoadingComponent : ComponentBase
{
    [Parameter, EditorRequired] public string? Item { get; set; }
}