using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotifyApi.Models
{
    public class PagedObject<T>
    {
        public List<T> Items { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
        public int Total { get; set; }
    }
}
