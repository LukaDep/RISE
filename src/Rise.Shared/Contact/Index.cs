using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Contact
{
    /// <summary>
    /// Response wrappers for contact-related operations.
    /// </summary>
    public static partial class ContactResponse
    {
        /// <summary>
        /// Response containing a list of contacts.
        /// Used for retrieving contact information for various departments and services.
        /// </summary>
        public class Index
        {
            public IEnumerable<ContactDto.Index> Contact { get; set; } = [];
        }
    }
}


