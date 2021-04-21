using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotifyApi.Models
{
    public class SearchPlaylist
    {
        public List<PlaylistItem> Items { get; set; }
    }
}
