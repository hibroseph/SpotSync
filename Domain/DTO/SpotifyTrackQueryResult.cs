using SpotSync.Domain.Contracts.SpotifyApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.DTO
{
    public class SpotifyTrackQueryResult : ISpotifyQueryResult
    {
        public string Uri { get; set; }
        public bool Explicit { get; set; }
        public string Name { get; set; }
        public List<Artist> Artists { get; set; }
        public string Album { get; set; }
        public double Length { get; set; }
    }
}
