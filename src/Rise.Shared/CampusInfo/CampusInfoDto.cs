using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.CampusInfo
{
    public static class CampusInfoDto
    {
        public class Index
        {
            public required string Id { get; set; }
            public required string Name { get; set; }
            public required string Location { get; set; }
            public required List<string> Faculties { get; set; } 
            public string ContactPhone { get; set; }
            public required string Description { get; set; }
        }
    }
}
