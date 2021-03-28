using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Classes.Responses.Party
{
    public class JoinedParty
    {
        public bool SuccessfullyJoinedParty { get; set; }
        public string PartyCode { get; set; }
    }
}
