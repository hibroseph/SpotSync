using SpotSync.Domain.PartyAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Models.Party
{
    public class UserContribution
    {
        public ContributionType Type { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
