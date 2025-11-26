using Rise.Shared.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Events
{
    public static partial class EventResponse
    {
        public class Index
        {
            public IEnumerable<EventDTO.Index> Event { get; set; } = [];
        }
    }
}
