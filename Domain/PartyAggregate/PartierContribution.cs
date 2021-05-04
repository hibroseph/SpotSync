using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.PartyAggregate
{
    public class PartierContribution
    {
        public ContributionType Type { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public Guid ContributionId { get; set; }
    }
}
