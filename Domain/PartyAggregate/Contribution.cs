using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.PartyAggregate
{
    public class Contribution
    {
        public ContributionType Type { get; set; }
        public IContribution Item { get; set; }
        public Guid Id { get; set; }
        public string ContributedBy { get; set; }
    }
}
