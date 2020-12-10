using SpotSync.Domain.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.DTO
{
    public class SpotifyArtistQueryResult : ISpotifyQueryResult
    {
        public string Uri { get; set; }
        public string Name { get; set; }
    }
}
