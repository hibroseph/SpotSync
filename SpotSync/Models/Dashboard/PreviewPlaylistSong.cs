using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Models.Dashboard
{
    public class PreviewPlaylistSong
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string TrackUri { get; set; }
        public bool Selected { get; set; }
    }
}
