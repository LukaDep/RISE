using Microsoft.AspNetCore.Components;


namespace Rise.Client.Contact.Components
{
    /// <summary>
    /// Contact card component that displays contact summary information.
    /// Shows name, type, and click handler for details.
    /// </summary>
    public partial class ContactCard : ComponentBase
    {
        /// <summary>Navigation manager for routing.</summary>
        [Inject] private NavigationManager Navigation { get; set; }
        
        /// <summary>Unique identifier of the contact.</summary>
        [Parameter, EditorRequired] public string Id { get; set; }
        
        /// <summary>Name of the contact.</summary>
        [Parameter, EditorRequired] public string Name { get; set; }
        
        /// <summary>Type/category of the contact.</summary>
        [Parameter, EditorRequired] public string Type { get; set; }
        
        /// <summary>Optional phone number.</summary>
        [Parameter] public string PhoneNumber { get; set; }
        
        /// <summary>Optional email address.</summary>
        [Parameter] public string Email { get; set; }

        /// <summary>Callback when card is clicked.</summary>
        [Parameter] public EventCallback OnClick { get; set; }
    }
}