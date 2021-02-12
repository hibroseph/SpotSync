using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain
{
    // There is a contract relationship with AddToQueueRequest
    public class Track : ValueObject
    {
        public string Name { get; set; }
        public string Artist { get; set; }
        public int Length { get; set; }
        public string Uri { get; set; }
        public string AlbumImageUrl { get; set; }
        public bool Explicit { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Artist;
            yield return Length;
            yield return Uri;
        }
    }
}
