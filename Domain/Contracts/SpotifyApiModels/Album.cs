using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotifyApiModels
{
    public class Album
    {
        public string Id { get; set; }
        public List<Image> Images { get; set; }
        public string Name { get; set; }
    }
}
