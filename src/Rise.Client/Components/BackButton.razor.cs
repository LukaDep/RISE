using Microsoft.AspNetCore.Components;

namespace Rise.Client.Components
{
    public partial class BackButton : ComponentBase
    {
        /// <summary>
        /// The text to display on the button. If null or empty, only the icon will be shown.
        /// </summary>
        [Parameter] public string? Text { get; set; }

        /// <summary>
        /// URL to navigate to.
        /// </summary>
        [Parameter, EditorRequired] public required string NavigateTo { get; set; }
    }
}
