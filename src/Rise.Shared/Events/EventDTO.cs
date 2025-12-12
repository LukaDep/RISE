using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Events
{
    /// <summary>
    /// Data transfer objects for events.
    /// </summary>
    public static class EventDTO
    {
        /// <summary>
        /// Represents an event for display and retrieval.
        /// Contains event details including type, name, time period, location, and registration information.
        /// </summary>
        public class Index
        {
            public required string Type { get; set; } = null!;
            public required string Name { get; set; } = null!;
            public required DateTime StartDateTime { get; set; }
            public required DateTime EndDateTime { get; set; }
            public string? Location { get; set; }
            public string? RegistrationLink { get; set; }
            public string? Description { get; set; }
        }
    }
}
