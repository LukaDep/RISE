using Microsoft.AspNetCore.Components;

namespace Rise.Client.Components
{
    public partial class FilterLabels : ComponentBase
    {
        [Parameter, EditorRequired] public List<string> Filters { get; set; }


    }
}
