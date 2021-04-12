using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Infrastructure.SpotifyApi
{
    class StartUserPlaybackSong
    {
        public List<string> uris { get; set; }
        public int position_ms { get; set; }
    }
}
