using Microsoft.AspNetCore.Components;

namespace Rise.Client.Components;

public partial class BasePage : ComponentBase
{
    /// <summary>
    /// The title displayed in the page header.
    /// </summary>
    [Parameter]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional content to be rendered in the header next to the title.
    /// </summary>
    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    /// <summary>
    /// The content to be rendered within the page layout.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
