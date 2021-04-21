using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SpotSync.Domain.Contracts.SpotifyApi.Models
{
    public class ExplicitContentSettings
    {
        [JsonPropertyName("filter_enabled")]
        public bool Filter { get; set; }
    }
}
