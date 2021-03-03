using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Models.Party
{
    public class TopPartyModel
    {
        public int ListenerCount { get; set; }
        public string PartyCode { get; set; }
        public SimpleTrackModel CurrentSong { get; set; }
    }
}
