using Microsoft.AspNetCore.Components;


namespace Rise.Client.Contact.Components
{
        public partial class ContactCard : ComponentBase
    {
        [Inject] private NavigationManager Navigation { get; set; }
        [Parameter, EditorRequired] public string Id { get; set; }
        [Parameter, EditorRequired] public string Name { get; set; }
        [Parameter, EditorRequired] public string Type { get; set; }
        [Parameter] public string PhoneNumber { get; set; }
        [Parameter] public string Email { get; set; }

    }
}