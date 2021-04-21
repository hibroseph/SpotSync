using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotifyApi.Models
{
    public class PlaylistItems
    {
        public List<PlaylistItem> Items { get; set; }
    }
}
