using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SpotSync.Domain.Contracts.SpotibroModels
{
    public class ArtistInformation
    {
        public Artist Artist { get; set; }
        public List<Track> TopTracks { get; set; }
    }
}
