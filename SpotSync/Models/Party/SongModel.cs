using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Models.Party
{
    public class SongModel
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string TrackUri { get; set; }
        public int Length { get; set; }
        public string AlbumImageUrl { get; set; }
    }
}
