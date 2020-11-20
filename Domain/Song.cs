using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain
{
    public class Song : ValueObject
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public int Length { get; set; }
        public string TrackUri { get; set; }
        public string AlbumImageUrl { get; set; }
        public bool Explicit { get; set; }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Title;
            yield return Artist;
            yield return Length;
            yield return TrackUri;
        }
    }
}
