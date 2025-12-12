using Microsoft.AspNetCore.Components;

namespace Rise.Client.Contact.Components
{
    /// <summary>
    /// Contact card details component that displays full contact information.
    /// Shows all contact fields with close functionality.
    /// </summary>
    public partial class ContactCardDetails : ComponentBase
    {
        /// <summary>Name of the contact.</summary>
        [Parameter, EditorRequired] public string Name { get; set; }
        
        /// <summary>Type/category of the contact.</summary>
        [Parameter, EditorRequired] public string Type { get; set; }
        
        /// <summary>Optional phone number.</summary>
        [Parameter] public string PhoneNumber { get; set; }
        
        /// <summary>Optional email address.</summary>
        [Parameter] public string Email { get; set; }
        
        /// <summary>Optional contact person name.</summary>
        [Parameter] public string ContactPerson { get; set; }

        /// <summary>Callback when details panel is closed.</summary>
        [Parameter] public EventCallback OnClose { get; set; }

    }
}
