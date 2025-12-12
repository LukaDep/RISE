using Rise.Shared.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Events
{
    /// <summary>
    /// Response wrappers for event-related operations.
    /// </summary>
    public static partial class EventResponse
    {
        /// <summary>
        /// Response containing a list of events.
        /// Used for retrieving and displaying upcoming or filtered events.
        /// </summary>
        public class Index
        {
            public IEnumerable<EventDTO.Index> Event { get; set; } = [];
        }
    }
}
