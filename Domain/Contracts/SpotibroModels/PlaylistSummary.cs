using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotibroModels
{
    public class PlaylistSummary
    {
        public string Name { get; set; }
        public string PlaylistCoverArtUrl { get; set; }
        public string Id { get; set; }
    }
}
