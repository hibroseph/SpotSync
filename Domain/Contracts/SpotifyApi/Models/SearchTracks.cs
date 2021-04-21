using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotifyApi.Models
{
    public class SearchTracks
    {
        public List<Track> Items { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
    }
}
