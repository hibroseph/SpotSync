using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.DTO
{
    public class CurrentSongDTO
    {
        public int ProgressMs { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string TrackUri { get; set; }
        public string AlbumArtUrl { get; set; }
    }
}
