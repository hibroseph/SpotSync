using SpotSync.Domain.Contracts.SpotifyApiModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SpotSync.Domain
{
    public class ArtistInformation
    {
        public Contracts.SpotifyApiModels.ArtistInformation Artist { get; set; }
        public List<Contracts.SpotifyApiModels.Track> TopTracks { get; set; }
    }
}
