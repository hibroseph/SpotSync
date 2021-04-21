using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotifyApi.Models
{
    public class SearchResults
    {
        public SearchArtists Artists { get; set; }
        public SearchTracks Tracks { get; set; }
    }
}
