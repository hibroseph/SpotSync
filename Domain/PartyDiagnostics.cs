using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain
{
    public class PartyDiagnostics
    {
        public PartyGoer Host { get; set; }
        public List<PartyGoer> Listeners { get; set; }
        public string PartyCode { get; set; }
        public List<Track> Queue { get; set; }
        public List<Track> History { get; set; }
        public Track CurrentTrack { get; set; }

        public List<PartyGoer> UsersThatHaveRequestedSkip { get; set; }
        public List<string> LikedSongs { get; set; }
    }
}
