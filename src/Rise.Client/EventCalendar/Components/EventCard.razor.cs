using Microsoft.AspNetCore.Components;
using Rise.Shared.Events;

namespace Rise.Client.EventCalendar.Components
{
    /// <summary>
    /// Event card component that displays event summary.
    /// Shows event title, date, and type badge.
    /// </summary>
    public partial class EventCard : ComponentBase
    {
        /// <summary>The event data to display.</summary>
        [Parameter, EditorRequired] public EventDTO.Index specificEvent { get; set; }
        
        /// <summary>Callback when card is clicked.</summary>
        [Parameter] public EventCallback OnClick { get; set; }
    }
}
