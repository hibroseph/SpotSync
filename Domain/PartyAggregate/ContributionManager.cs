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

        public void AddContributions(List<Contribution> contribution)
        {
            _contributions.AddRange(contribution);
        }

        public void RemoveContributions(List<Contribution> contributions)
        {
            foreach (var contribution in contributions)
            {
                _contributions.RemoveAll(p => p.ContributedBy == contribution.ContributedBy && p.Id == contribution.Id);
            }
        }

        public List<PartierContribution> GetContributions(PartyGoer partier)
        {
            return _contributions.Where(p => p.ContributedBy == partier.GetId()).Select(p => new PartierContribution
            {
                Id = p.Id,
                ContributionId = p.ContributionId,
                Name = p.Name,
                Type = p.Type
            }).ToList();
        }

        public void RemoveContribution(PartyGoer partier, Guid contributionId)
        {
            _contributions.RemoveAll(p => p.ContributedBy == partier.GetId() && p.ContributionId == contributionId);
        }

        public Dictionary<ContributionType, List<string>> GetContributionSeeds(List<PartyGoer> partyGoers)
        {
            Dictionary<ContributionType, List<string>> seedContributions = new Dictionary<ContributionType, List<string>>();

            if (partyGoers != null && partyGoers.Count == 0 || _contributions.Count == 0)
            {
                return seedContributions;
            }

            HashSet<string> selectedPartyGoers = new HashSet<string>();

            Dictionary<ContributionType, List<int>> alreadyGrabbedContributions = new Dictionary<ContributionType, List<int>>();

            bool canSelectPartyGoersMultipleTimes = partyGoers.Count < 5;
            int addedContributions = 0;
            bool exit = false;

            while (seedContributions.Count < 5 && !exit)
            {
                string selectedPartyGoerId = partyGoers.ElementAt(_random.Next(0, partyGoers.Count)).GetId();

                Contribution possibleContribution = _contributions.ElementAt(_random.Next(0, _contributions.Count));

                if (canSelectPartyGoersMultipleTimes || !selectedPartyGoers.Contains(selectedPartyGoerId))
                {
                    if (!canSelectPartyGoersMultipleTimes)
                    {
                        selectedPartyGoers.Add(selectedPartyGoerId);
                    }

                    if (seedContributions.ContainsKey(possibleContribution.Type))
                    {
                        seedContributions[possibleContribution.Type].Add(possibleContribution.Id);
                    }
                    else
                    {
                        seedContributions.Add(possibleContribution.Type, new List<string> { possibleContribution.Id });
                    }

                    addedContributions++;
                }

                if (addedContributions >= _contributions.Count || addedContributions == 5)
                {
                    exit = true;
                }
            }

            return seedContributions;
        }
    }
}
