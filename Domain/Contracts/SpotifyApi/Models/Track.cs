using SpotSync.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SpotSync.Domain.Contracts.SpotifyApi.Models
{
    public class Track
    {
        public string Id { get; set; }
        public List<Artist> Artists { get; set; }
        public Album Album { get; set; }

        [JsonPropertyName("duration_ms")]
        public int Duration { get; set; }
        public bool Explicit { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("available_markets")]
        public List<string> Markets { get; set; }
    }
}
