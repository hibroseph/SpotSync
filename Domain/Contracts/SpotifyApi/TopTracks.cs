using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotifyApi
{

    public class TopTracks
    {
        public List<Contracts.SpotifyApi.Models.Track> Tracks { get; set; }
    }

}
