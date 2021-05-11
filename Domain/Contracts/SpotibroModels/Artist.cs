using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotibroModels
{
    public class Artist : IUnique
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<Image> Images { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
