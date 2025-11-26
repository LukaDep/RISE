using Microsoft.AspNetCore.Components;
using Rise.Shared.Events;

namespace Rise.Client.EventCalendar.Components
{
    public partial class EventCardDetails : ComponentBase
    {
        [Parameter, EditorRequired] public EventDTO.Index specificEvent { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
    }
}
