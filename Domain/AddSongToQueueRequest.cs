using SpotSync.Domain.Contracts.SpotifyApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain
{
    // There is a contract relationship with track
    public class AddSongToQueueCommand
    {
        public string Name { get; set; }
        public List<Artist> Artists { get; set; }
        public string TrackUri { get; set; }
        public int Length { get; set; }
        public string AlbumImageUrl { get; set; }
        public string PartyCode { get; set; }
        public int IndexToInsertSongAt { get; set; }
        public string AddedBy { get; set; }
        public bool Explicit { get; set; }
    }
}
