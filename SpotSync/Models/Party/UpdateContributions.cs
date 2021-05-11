using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Models.Party
{
    public class UpdateContributions
    {
        public List<UserContribution> NewContributions { get; set; }
        public List<UserContribution> ContributionsToRemove { get; set; }
    }
}
