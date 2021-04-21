using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotibroModels
{
    public class Artist
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<Image> Images { get; set; }
    }
}
