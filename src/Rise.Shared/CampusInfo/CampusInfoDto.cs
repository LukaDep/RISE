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
            public required Guid Id { get; set; }
            public required string Name { get; set; } //niet
            public required string Location { get; set; }//niey
            public required List<string> Faciliteiten { get; set; }//mqg weg
            public string ContactPhone { get; set; }//mee
            public required string Description { get; set; }//mee
        }
    }
}
