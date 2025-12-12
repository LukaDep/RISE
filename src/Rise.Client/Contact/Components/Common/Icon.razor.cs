using Microsoft.AspNetCore.Components;

namespace Rise.Client.Contact.Components.Common
{
    /// <summary>
    /// Icon component that renders SVG icons by type.
    /// Maps icon type names to their SVG representations.
    /// </summary>
    public partial class Icon
    {
        /// <summary>The type of icon to render (e.g., "phone", "email").</summary>
        [Parameter, EditorRequired] public string Type { get; set; }
    }
}
