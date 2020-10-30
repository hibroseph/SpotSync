using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain
{
    public class AddSongToQueueRequest
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string TrackUri { get; set; }
        public int Length { get; set; }
        public string AlbumImageUrl { get; set; }
        public string PartyCode { get; set; }
        public int IndexToInsertSongAt { get; set; }
    }
}
