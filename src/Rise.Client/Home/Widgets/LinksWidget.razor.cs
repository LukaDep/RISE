using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Rise.Client.Home.Widgets;

public partial class LinksWidget : ComponentBase
{
    [Parameter] public EventCallback<Guid> OnRemove { get; set; }
    [Parameter] public bool EditMode { get; set; }
    [Parameter] public int Index { get; set; }
    [Parameter] public Guid WidgetId { get; set; }
    [Inject] public IJSRuntime Js { get; set; } = default!;

}
