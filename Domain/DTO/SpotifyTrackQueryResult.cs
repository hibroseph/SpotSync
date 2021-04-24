using SpotSync.Domain.Contracts.SpotifyApi.Models;
using System.Collections.Generic;

namespace SpotSync.Domain.DTO
{
    public class SpotifyTrackQueryResult : ISpotifyQueryResult
    {
        public string Id { get; set; }
        public bool Explicit { get; set; }
        public string Name { get; set; }
        public List<Artist> Artists { get; set; }
        public string Album { get; set; }
        public double Duration { get; set; }
    }
}
