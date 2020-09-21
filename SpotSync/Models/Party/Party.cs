using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Models.Party
{
    public class Party
    {
        public string PartyCode { get; set; }
        public List<string> Attendees { get; set; }
    }
}
