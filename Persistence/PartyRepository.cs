using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using SpotSync.Domain.Errors;

namespace Persistence
{
    public class PartyRepository : IPartyRepository
    {
        // TODO: implement this as a dictionary for faster lookups
        private List<Party> _parties;

        public PartyRepository()
        {
            _parties = new List<Party>();
        }

        public void CreateParty(Party item)
        {
            _parties.Add(item);
        }

        public bool Exists(Party item)
        {
            return _parties.Contains(item);
        }

        public Party Get(Party item)
        {
            return _parties.Find(p => p.GetPartyCode() == item.GetPartyCode());
        }

        public void Update(Party party)
        {
            var parties = _parties.FindAll(p => p.GetPartyCode() == party.GetPartyCode());

            if (parties.Count != 1)
            {
                throw new Exception($"There was {parties.Count} returned when searched for a party code{party.GetPartyCode()}");
            }

            Party partyToBeUpdated = parties.First();

            partyToBeUpdated = party;
        }

        public Task<Party> GetAsync(PartyCodeDTO partyCode)
        {
            return Task.FromResult(_parties.Find(p => p.GetPartyCode().Equals(partyCode.PartyCode, StringComparison.OrdinalIgnoreCase)));
        }

        public void Remove(Party item)
        {
            throw new NotImplementedException();
        }

        public bool IsUserHostingAParty(PartyGoer host)
        {
            return _parties.Find(p => p.IsHost(host)) != null;
        }

        public Task<Party> GetPartyWithHostAsync(PartyGoer host)
        {
            return Task.FromResult(_parties.Find(p => p.IsHost(host)));
        }

        public bool Delete(PartyGoer host)
        {
            List<Party> parties = _parties.FindAll(p => p.IsHost(host));

            if (parties.Count > 1)
            {
                throw new Exception($"Host: {host?.Id} is hosting {parties.Count} parties. A host should only host 1 party at a time.");
            }

            if (parties == null)
            {
                throw new Exception($"Host: {host?.Id} is not hosting a party");
            }

            _parties.Remove(parties.First());

            return true;
        }

        public Task<bool> DeleteAsync(string partyCode)
        {
            _parties.RemoveAll(p => p.GetPartyCode().Equals(partyCode, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(true);
        }

        public Task<bool> IsUserInAPartyAsync(PartyGoer attendee)
        {
            return Task.FromResult(_parties.Find(p => p.IsListener(attendee)) != null);
        }

        public Task<Party> GetPartyWithAttendeeAsync(PartyGoer attendee)
        {
            return Task.FromResult(_parties.Find(p => p.IsListener(attendee)));
        }

        public bool LeaveParty(PartyGoer attendee)
        {
            List<Party> parties = _parties.FindAll(p => p.IsListener(attendee));

            if (parties.Count > 1)
            {
                parties.ForEach(p => p.LeaveParty(attendee));

                throw new PartyGoerWasInMultiplePartiesException($"The user {attendee} was in {parties.Count} but was successfully removed from them");
            }

            if (parties == null || parties.Count == 0)
            {
                throw new Exception($"The attendee: {attendee.Id} is not currently in a party");
            }

            parties.First().LeaveParty(attendee);

            return true;
        }

        public Task<List<Party>> GetPartiesWithMostListenersAsync(int count)
        {
            _parties.Sort((r, l) => r.GetListenerCount() > l.GetListenerCount() ? 1 : 0);

            return Task.FromResult(_parties.Take(count).ToList());
        }

        public Task<Party> GetPartyWithCodeAsync(string partyCode)
        {
            List<Party> parties = _parties.FindAll(p => p.GetPartyCode().Equals(partyCode, StringComparison.OrdinalIgnoreCase));

            if (parties.Count > 1)
            {
                throw new Exception($"There is more than 1 party with the same party code of {partyCode}");
            }

            if (parties.Count == 1)
            {
                return Task.FromResult(parties.First());
            }

            return Task.FromResult<Party>(null);
        }

        public Task RemoveHostFromPartyAsync(PartyGoer host)
        {
            _parties.ForEach(p =>
            {
                if (p.IsHost(host))
                {
                    p.LeaveParty(host);
                }
            });

            return Task.CompletedTask;
        }
    }
}
