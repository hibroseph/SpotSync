using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotibroModels
{
    public class Track : IUnique
    {
        public string Id { get; set; }
        public List<Artist> Artists { get; set; }
        public int Duration { get; set; }
        public bool IsExplicit { get; set; }
        public Album Album { get; set; }
        public string Name { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
