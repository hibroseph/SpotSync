using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.DTO
{
    public class GetRecommendedSongs
    {
        public List<string> SeedTrackUris { get; set; }
        public string Market { get; set; }
    }
}
