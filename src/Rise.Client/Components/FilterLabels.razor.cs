using Microsoft.AspNetCore.Components;

namespace Rise.Client.Components
{
    /// <summary>
    /// Component that renders a collection of filter label chips.
    /// Displays multiple FilterLabel components for a list of filters.
    /// </summary>
    public partial class FilterLabels : ComponentBase
    {
        /// <summary>The list of filter values to display.</summary>
        [Parameter, EditorRequired] public List<string> Filters { get; set; }


    }
}
