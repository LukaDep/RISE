using Microsoft.AspNetCore.Components;

namespace Rise.Client.Contact.Components
{
    public partial class ContactCardDetails : ComponentBase
    {
        [Parameter, EditorRequired] public string Name { get; set; }
        [Parameter, EditorRequired] public string Type { get; set; }
        [Parameter] public string PhoneNumber { get; set; }
        [Parameter] public string Email { get; set; }
        [Parameter] public string ContactPerson { get; set; }

        [Parameter] public EventCallback OnClose { get; set; }

    }
}
