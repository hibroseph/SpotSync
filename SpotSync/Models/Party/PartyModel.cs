
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace SpotSync.Models.Party
{
    public class PartyModel
    {
        public bool IsCurrentlyHostingParty { get; set; }
        public bool IsCurrentlyJoinedInAParty { get; set; }
        public Party CurrentlyActiveParty { get; set; }
    }
}
