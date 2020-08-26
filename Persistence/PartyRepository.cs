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
        private List<Party> Parties = new List<Party>();
        public void CreateParty(Party item)
        {
            Parties.Add(item);
        }

        public bool Exists(Party item)
        {
            return Parties.Exists(party => party.Id == item.Id && party.PartyCode == item.PartyCode);
        }

        public Party Get(Party item)
        {
            return Parties.Find(p => p.Id == item.Id);
        }

        public Party Get(Guid partyId)
        {
            return Parties.Find(p => p.Id == partyId);
        }

        public void Update(Party party)
        {
            var parties = Parties.FindAll(p => p.Id == party.Id);

            if (parties.Count != 1)
            {
                throw new Exception($"There was {parties.Count} returned when searched for a party with ID {party.Id}");
            }

            Party partyToBeUpdated = parties.First();

            partyToBeUpdated = party;
        }

        public Task<Party> GetAsync(PartyCodeDTO partyCode)
        {
            return Task.FromResult(Parties.Find(p => p.PartyCode == partyCode.PartyCode));
        }

        public void Remove(Party item)
        {
            throw new NotImplementedException();
        }

        public bool IsUserHostingAParty(PartyGoer host)
        {
            return Parties.Find(p => p.Host.Id == host.Id) != null;
        }

        public Task<Party> GetPartyWithHostAsync(PartyGoer host)
        {
            return Task.FromResult(Parties.Find(p => p.Host.Id == host.Id));
        }

        public Task<bool> DeleteAsync(PartyGoer host)
        {
            try
            {
                Parties.RemoveAll(p => p.Host.Id.Equals(host.Id, StringComparison.CurrentCultureIgnoreCase));

                return Task.FromResult(true);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> IsUserInAPartyAsync(PartyGoer attendee)
        {
            try
            {
                return Task.FromResult(Parties.Find(p => p.Attendees.Exists(p => p.Id == attendee.Id)) != null);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        public Task<Party> GetPartyWithAttendeeAsync(PartyGoer attendee)
        {
            return Task.FromResult(Parties.Find(p => p.Attendees.Exists(p => p.Id == attendee.Id)));
        }

        public Task<bool> LeavePartyAsync(PartyGoer attendee)
        {
            try
            {
                Parties.ForEach(p => p.Attendees.RemoveAll(p => p.Id == attendee.Id));

                return Task.FromResult(true);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }
    }
}
