using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotifyApi.Models
{
    public class Image
    {
        public int? Height { get; set; }
        public int? Width { get; set; }
        public string Url { get; set; }
    }
}
