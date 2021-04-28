using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.PartyAggregate
{
    public class Contribution
    {
        public ContributionType Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public Guid ContributionId { get; set; }
        public string ContributedBy { get; set; }
    }
}
