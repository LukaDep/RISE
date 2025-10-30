using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Contact
{
    public static partial class ContactResponse
    {
        public class Index
        {
            public IEnumerable<ContactDto.Index> Contact { get; set; } = [];
        }
    }
}


