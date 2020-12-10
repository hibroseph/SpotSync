using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.DTO
{
    public class SpotifyAlbumQueryResult : ISpotifyQueryResult
    {
        public string Uri { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
    }
}
