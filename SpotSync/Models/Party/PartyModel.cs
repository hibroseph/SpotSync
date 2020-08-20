
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Models.Party
{
    public class PartyModel
    {
        public bool IsCurrentlyHostingParty { get; set; }
        public Party CurrentlyActiveParty { get; set; }
    }
}
