using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Models.Dashboard
{
    public class PreviewPartyModel
    {
        public string Name { get; set; }
        public string AlbumArtUrl { get; set; }
        public int ListenerCount { get; set; }
        public string PartyCode { get; set; }
    }
}
