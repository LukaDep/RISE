using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Contact
{
    public static partial class ContactResponse
    {
        public class Get
        {
            public required ContactDto.Index ContactItem { get; set; }
        }
    }

}

