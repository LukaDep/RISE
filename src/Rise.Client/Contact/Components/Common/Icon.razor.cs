using Microsoft.AspNetCore.Components;

namespace Rise.Client.Contact.Components.Common
{
    public partial class Icon
    {
        [Parameter, EditorRequired] public string Type { get; set; }
    }
}
