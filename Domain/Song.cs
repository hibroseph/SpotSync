using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain
{
    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public int Length { get; set; }
        public string TrackUri { get; set; }
    }
}
