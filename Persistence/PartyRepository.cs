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
            return _parties.Exists(party => party.Id == item.Id && party.PartyCode == item.PartyCode);
        }

        public Party Get(Party item)
        {
            return _parties.Find(p => p.Id == item.Id);
        }

        public Party Get(Guid partyId)
        {
            return _parties.Find(p => p.Id == partyId);
        }

        public void Update(Party party)
        {
            var parties = _parties.FindAll(p => p.Id == party.Id);

            if (parties.Count != 1)
            {
                throw new Exception($"There was {parties.Count} returned when searched for a party with ID {party.Id}");
            }

            Party partyToBeUpdated = parties.First();

            partyToBeUpdated = party;
        }

        public Task<Party> GetAsync(PartyCodeDTO partyCode)
        {
            return Task.FromResult(_parties.Find(p => p.PartyCode.Equals(partyCode.PartyCode, StringComparison.OrdinalIgnoreCase)));
        }

        public void Remove(Party item)
        {
            throw new NotImplementedException();
        }

        public bool IsUserHostingAParty(PartyGoer host)
        {
            return _parties.Find(p => p.Host == host) != null;
        }

        public Task<Party> GetPartyWithHostAsync(PartyGoer host)
        {
            return Task.FromResult(_parties.Find(p => p.Host == host));
        }

        public async Task<bool> DeleteAsync(PartyGoer host)
        {
            List<Party> parties = _parties.FindAll(p => p.Host == host);

            if (parties.Count > 1)
            {
                throw new Exception($"Host: {host?.Id} is hosting {parties.Count} parties. A host should only host 1 party at a time.");
            }

            if (parties == null)
            {
                throw new Exception($"Host: {host?.Id} is not hosting a party");
            }

            if (parties.First().Playlist != null)
            {
                await parties.First().Playlist?.DeleteAsync();
            }
            _parties.Remove(parties.First());

            return true;
        }

        public Task<bool> DeleteAsync(string partyCode)
        {
            _parties.RemoveAll(p => p.PartyCode.Equals(partyCode, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(true);
        }

        public Task<bool> IsUserInAPartyAsync(PartyGoer attendee)
        {
            return Task.FromResult(_parties.Find(p => p.Listeners.Exists(p => p.Id == attendee.Id)) != null);
        }

        public Task<Party> GetPartyWithAttendeeAsync(PartyGoer attendee)
        {
            return Task.FromResult(_parties.Find(p => p.Listeners.Exists(p => p.Id == attendee.Id)));
        }

        public bool LeaveParty(PartyGoer attendee)
        {
            List<Party> parties = _parties.FindAll(p => p.Listeners.Contains(attendee));

            if (parties.Count > 1)
            {
                parties.ForEach(p => p.Listeners.RemoveAll(p => p == attendee));

                throw new PartyGoerWasInMultiplePartiesException($"The user {attendee} was in {parties.Count} but was successfully removed from them");
            }

            if (parties == null || parties.Count == 0)
            {
                throw new Exception($"The attendee: {attendee.Id} is not currently in a party");
            }

            if (parties.First().Playlist != null)
            {
                parties.First().Playlist.RemoveListener(attendee);
            }

            parties.First().Listeners.RemoveAll(p => p.Id.Equals(attendee.Id, StringComparison.OrdinalIgnoreCase));

            return true;
        }

        public Task<List<Party>> GetPartiesWithMostListenersAsync(int count)
        {
            _parties.Sort((r, l) => r.Listeners.Count > l.Listeners.Count ? 1 : 0);

            return Task.FromResult(_parties.Take(count).ToList());
        }

        public Task<Party> GetPartyWithCodeAsync(string partyCode)
        {
            List<Party> parties = _parties.FindAll(p => p.PartyCode.Equals(partyCode, StringComparison.OrdinalIgnoreCase));

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
                if (p.Host == host && p.Listeners.Count > 1)
                {
                    p.Host = p.Listeners.First();
                }
                else
                {
                    p.Host = null;
                }
            });

            return Task.CompletedTask;
        }
    }
}
