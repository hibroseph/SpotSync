using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain
{
    // There is a contract relationship with AddToQueueRequest
    public class Track : ValueObject
    {
        public string Name { get; set; }
        public List<SpotSync.Domain.Contracts.SpotifyApi.Models.Artist> Artists { get; set; }
        public int Length { get; set; }
        public string Id { get; set; }
        public string AlbumImageUrl { get; set; }
        public bool Explicit { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Artists;
            yield return Length;
            yield return Id;
        }
    }
}
