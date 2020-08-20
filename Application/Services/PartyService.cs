using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Application.Services
{
    public class PartyService : IPartyService
    {
        IPartyRepository _partyRepository;
        IPartyGoerService _partyGoerService;
        ISpotifyHttpClient _spotifyHttpClient;

        public PartyService(IPartyRepository partyRepository, IPartyGoerService partyGoerService, ISpotifyHttpClient spotifyHttpClient)
        {
            _partyRepository = partyRepository;
            _spotifyHttpClient = spotifyHttpClient;
            _partyGoerService = partyGoerService;
        }

        public bool IsUserHostingAParty(PartyGoer host)
        {
            return _partyRepository.IsUserHostingAParty(host);
        }

        public async Task<bool> EndPartyAsync(PartyGoer host)
        {
            return await _partyRepository.DeleteAsync(host);
        }

        public async Task<bool> JoinPartyAsync(PartyCodeDTO partyCode, PartyGoer attendee)
        {
            Party party = await _partyRepository.GetAsync(partyCode);

            if (party == null)
            {
                return false;
            }

            return await JoinPartyAsync(party.Id, attendee);
        }
        private Task<bool> JoinPartyAsync(Guid partyId, PartyGoer attendee)
        {
            try
            {
                Party party = _partyRepository.Get(partyId);

                party.JoinParty(attendee);

                _partyRepository.Update(party);

                return Task.FromResult(true);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        public string StartNewParty(PartyGoer partyHost)
        {
            Party party = new Party(partyHost);

            _partyRepository.Add(party);

            return party.PartyCode;
        }

        public async Task<bool> UpdateSongForEveryoneInPartyAsync(Party party, PartyGoer host)
        {
            try
            {
                if (party is null)
                {
                    throw new Exception($"Obtaining a party with ID {party.Id} returned null from the database");
                }

                if (!party.Host.Id.Equals(host.Id, StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new Exception($"A non host tried to change the song for the party with ID {party.Id}. Attempted host ID: {host.Id}");
                }

                // Get the current song from the host
                CurrentSongDTO song = await _spotifyHttpClient.GetCurrentSongAsync(host.Id);

                List<Task<bool>> updateSongForPartyTask = new List<Task<bool>>();
                foreach (PartyGoer attendee in party.Attendees)
                {
                    updateSongForPartyTask.Add(_spotifyHttpClient.UpdateSongForPartyGoerAsync(attendee.Id, song));
                }

                await Task.WhenAll(updateSongForPartyTask);

                // Verify all the updates worked
                foreach (Task<bool> task in updateSongForPartyTask)
                {
                    if (!task.Result)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<Party> GetPartyAsync(PartyCodeDTO partyCode)
        {
            return await _partyRepository.GetAsync(partyCode);
        }

        public async Task<Party> GetPartyAsync(PartyGoer host)
        {
            return await _partyRepository.GetAsync(host);
        }
    }
}
