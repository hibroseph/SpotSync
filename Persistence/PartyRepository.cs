using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.DTO;

namespace Persistence
{
    public class PartyRepository : IPartyRepository
    {
        private List<Party> _parties = new List<Party>();
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
            return _parties.Find(p => p.Host.Id == host.Id) != null;
        }

        public Task<Party> GetPartyWithHostAsync(PartyGoer host)
        {
            return Task.FromResult(_parties.Find(p => p.Host.Id == host.Id));
        }

        public async Task<bool> DeleteAsync(PartyGoer host)
        {
            try
            {
                //Parties.RemoveAll(p => p.Host.Id.Equals(host.Id, StringComparison.CurrentCultureIgnoreCase));

                List<Party> parties = _parties.FindAll(p => p.Host.Id.Equals(host.Id, StringComparison.CurrentCultureIgnoreCase));

                if (parties.Count > 1)
                {
                    throw new Exception($"Host: {host.Id} is hosting {parties.Count} parties. A host should only host 1 party at a time.");
                }

                if (parties == null)
                {
                    throw new Exception($"Host: {host.Id} is not hosting a party");
                }

                if (parties.First().Playlist != null)
                {
                    await parties.First().Playlist?.DeleteAsync();
                }
                _parties.Remove(parties.First());

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task<bool> IsUserInAPartyAsync(PartyGoer attendee)
        {
            try
            {
                return Task.FromResult(_parties.Find(p => p.Attendees.Exists(p => p.Id == attendee.Id)) != null);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        public Task<Party> GetPartyWithAttendeeAsync(PartyGoer attendee)
        {
            return Task.FromResult(_parties.Find(p => p.Attendees.Exists(p => p.Id == attendee.Id)));
        }

        public async Task<bool> LeavePartyAsync(PartyGoer attendee)
        {
            try
            {
                List<Party> parties = _parties.FindAll(p => p.Attendees.Contains(attendee));

                if (parties.Count > 1)
                {
                    throw new Exception($"The attendee: {attendee.Id} was in {parties.Count} parties");
                }

                if (parties == null || parties.Count == 0)
                {
                    throw new Exception($"The attendee: {attendee.Id} is not currently in a party");
                }

                if (parties.First().Playlist != null)
                {
                    await parties.First().Playlist.RemoveListener(attendee);
                }

                parties.First().Attendees.RemoveAll(p => p.Id.Equals(attendee.Id, StringComparison.OrdinalIgnoreCase));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
