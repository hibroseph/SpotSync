using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SpotSync.Domain.Contracts.SpotifyApiModels
{
    public class RecommendedTracks
    {
        [JsonPropertyName("tracks")]
        public List<Track> Tracks { get; set; }
    }
}
