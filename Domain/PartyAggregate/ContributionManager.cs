using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotSync.Domain.PartyAggregate
{
    class ContributionManager
    {
        private List<Contribution> _contributions;
        private Random _random;

        public ContributionManager()
        {
            _contributions = new List<Contribution>();
            _random = new Random();
        }

        public void AddContribution(Contribution contribution)
        {
            _contributions.Add(contribution);
        }

        public Dictionary<ContributionType, string> GetContributionSeeds(List<PartyGoer> partyGoers)
        {
            if (partyGoers != null && partyGoers.Count == 0)
            {
                return new Dictionary<ContributionType, string>();
            }

            // get 5 random items from all the contributions and try to spread it around the users
            Dictionary<ContributionType, string> seedContributions = new Dictionary<ContributionType, string>();

            HashSet<string> selectedPartyGoers = new HashSet<string>();

            bool canSelectPartyGoersMultipleTimes = partyGoers.Count < 5;

            bool exit = false;

            while (seedContributions.Count < 5 || exit)
            {
                string selectedPartyGoerId = partyGoers.ElementAt(_random.Next(0, partyGoers.Count)).GetId();

                Contribution possibleContribution = _contributions.ElementAt(_random.Next(0, _contributions.Count));

                if (canSelectPartyGoersMultipleTimes || !selectedPartyGoers.Contains(selectedPartyGoerId))
                {
                    seedContributions.Add(possibleContribution.Type, possibleContribution.Item.Id);
                }

                if (seedContributions.Count == _contributions.Count)
                {
                    exit = true;
                }
            }

            return seedContributions;
        }
    }
}
