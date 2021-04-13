using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SpotSync.Domain.DTO
{
    public class SpotifyUser
    {
        [JsonPropertyName("id")]
        public string SpotifyId { get; set; }
        [JsonPropertyName("explicit_content")]
        public SpotifyExplicitContentSettings ExplicitSettings { get; set; }
        [JsonPropertyName("country")]
        public string Market { get; set; }
        [JsonPropertyName("product")]
        public string Product { get; set; }
    }
}
