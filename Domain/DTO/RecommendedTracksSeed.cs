using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.DTO
{
    public class RecommendedTracksSeed
    {
        public List<string> SeedTrackUris { get; set; }
        public List<string> SeedArtistUris { get; set; }
        public string Market { get; set; }
    }
}
