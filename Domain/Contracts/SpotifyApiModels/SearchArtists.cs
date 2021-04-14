using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotifyApiModels
{
    public class SearchArtists
    {
        public List<Artist> Items { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
    }
}
