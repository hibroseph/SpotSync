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
using SpotSync.Domain.PartyAggregate;

namespace Persistence
{
    public class PartyRepository : IPartyRepository
    {
        // TODO: implement this as a dictionary for faster lookups
        private Dictionary<string, Party> _parties;

        public Task<List<Party>> GetAllPartiesAsync()
        {
            return Task.FromResult(_parties.Select(pair => pair.Value).ToList());
        }

        public PartyRepository()
        {
            _parties = new Dictionary<string, Party>();
        }

        public void CreateParty(Party party)
        {

            if (!_parties.ContainsKey(party.GetPartyCode()))
            {
                _parties.Add(party.GetPartyCode(), party);
                return;
            }

            throw new Exception($"A party with code {party.GetPartyCode()} was attempted to be created but one already existed");
        }

        public void UpdateParty(Party party)
        {
            if (_parties.ContainsKey(party.GetPartyCode()))
            {
                _parties[party.GetPartyCode()] = party;
                return;
            }

            throw new Exception("While trying to update an existing party, no party was returned");
        }

        public Party GetParty(string partyCode)
        {
            if (_parties.ContainsKey(partyCode))
            {
                return _parties[partyCode];
            }

            return null;
        }

        public bool IsUserHostingAParty(PartyGoer host)
        {
            foreach (var key in _parties.Keys)
            {
                if (_parties[key].IsHost(host))
                {
                    return true;
                }
            }

            return false;
        }

        public Task<Party> GetPartyWithHostAsync(PartyGoer host)
        {
            foreach (var key in _parties.Keys)
            {
                if (_parties[key].IsHost(host))
                {
                    return Task.FromResult(_parties[key]);
                }
            }

            return Task.FromResult<Party>(null);
        }

        public bool DeletePartyWithHost(PartyGoer host)
        {
            List<string> partyKeys = new List<string>();

            foreach (var key in _parties.Keys)
            {
                if (_parties[key].IsHost(host))
                {
                    partyKeys.Add(key);
                }
            }

            if (partyKeys.Count > 1)
            {
                throw new Exception($"Host: {host?.GetId()} is hosting {partyKeys.Count} parties. A host should only host 1 party at a time.");
            }

            if (partyKeys == null)
            {
                throw new Exception($"Host: {host?.GetId()} is not hosting a party");
            }

            _parties.Remove(partyKeys.First());

            return true;
        }

        public Task<bool> DeleteAsync(string partyCode)
        {
            if (_parties.ContainsKey(partyCode))
            {
                _parties[partyCode].EndParty();
                _parties.Remove(partyCode);
            }

            return Task.FromResult(true);
        }

        public Task<bool> IsUserInAPartyAsync(PartyGoer attendee)
        {
            foreach (var key in _parties.Keys)
            {
                if (_parties[key].IsListener(attendee))
                {
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }

        public Task<Party> GetPartyWithAttendeeAsync(PartyGoer attendee)
        {
            foreach (var key in _parties.Keys)
            {
                if (_parties[key].IsListener(attendee))
                {
                    return Task.FromResult(_parties[key]);
                }
            }

            return Task.FromResult<Party>(null);
        }

        public bool LeaveParty(PartyGoer attendee)
        {
            List<string> partyKeys = new List<string>();

            foreach (var key in _parties.Keys)
            {
                if (_parties[key].IsListener(attendee))
                {
                    partyKeys.Add(key);
                }
            }

            if (partyKeys.Count > 1)
            {
                partyKeys.ForEach(p => _parties[p].LeaveParty(attendee));

                throw new PartyGoerWasInMultiplePartiesException($"The user {attendee} was in {partyKeys.Count} but was successfully removed from them");
            }

            if (partyKeys == null || partyKeys.Count == 0)
            {
                throw new Exception($"The attendee: {attendee.GetId()} is not currently in a party");
            }

            _parties[partyKeys.First()].LeaveParty(attendee);

            return true;
        }

        public Task<List<Party>> GetPartiesWithMostListenersAsync(int count)
        {
            List<Party> partyList = _parties.Select(p => p.Value).ToList();
            partyList.Sort((r, l) => r.GetListenerCount() > l.GetListenerCount() ? 1 : 0);

            return Task.FromResult(partyList.Take(count).ToList());
        }

        public Task<Party> GetPartyWithCodeAsync(string partyCode)
        {
            if (_parties.ContainsKey(partyCode))
            {
                return Task.FromResult(_parties[partyCode]);
            }

            return Task.FromResult<Party>(null);
        }

        public Task RemoveHostFromPartyAsync(PartyGoer host)
        {
            foreach (string key in _parties.Keys)
            {
                if (_parties[key].IsHost(host))
                {
                    _parties[key].LeaveParty(host);
                }
            }

            return Task.CompletedTask;
        }
    }
}
