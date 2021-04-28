using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.PartyAggregate
{
    public interface IContribution
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
