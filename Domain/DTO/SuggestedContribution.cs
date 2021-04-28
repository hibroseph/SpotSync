using SpotSync.Domain.PartyAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.DTO
{
    public class SuggestedContribution
    {
        public string Id { get; set; }
        public ContributionType Type { get; set; }
        public string Name { get; set; }
    }
}
