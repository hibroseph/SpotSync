using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SpotSync.Domain.Contracts.SpotifyApi.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public string SpotifyId { get; set; }
        [JsonPropertyName("explicit_content")]
        public ExplicitContentSettings ExplicitSettings { get; set; }
        [JsonPropertyName("country")]
        public string Market { get; set; }
        [JsonPropertyName("product")]
        public string Product { get; set; }
    }
}
