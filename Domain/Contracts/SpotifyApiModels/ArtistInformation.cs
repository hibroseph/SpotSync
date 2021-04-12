using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotifyApiModels
{
    public class ArtistInformation
    {
        public string Name { get; set; }
        public List<Image> Images { get; set; }
        public List<string> Genres { get; set; }
        public string Id { get; set; }
    }
}
