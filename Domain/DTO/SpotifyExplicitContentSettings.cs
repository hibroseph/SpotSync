using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SpotSync.Domain.DTO
{
    public class SpotifyExplicitContentSettings
    {
        [JsonPropertyName("filter_enabled")]
        public bool Filter { get; set; }
    }
}
