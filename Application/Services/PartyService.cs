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
        ISpotifyHttpClient _spotifyHttpClient;
        Random _random;

        public PartyService(IPartyRepository partyRepository, ISpotifyHttpClient spotifyHttpClient)
        {
            _partyRepository = partyRepository;
            _spotifyHttpClient = spotifyHttpClient;
            _random = new Random();
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

            /*party.CreatePlaylist(new Playlist(new List<Song> { new Song { Length = 4000 }, new Song { Length = 8000 }, new Song { Length = 1000 } }));
            party.StartPlaylist();
            */
            _partyRepository.CreateParty(party);

            return party.PartyCode;
        }

        public bool NextSong(Song song)
        {

            return true;
        }

        public async Task<bool> UpdateCurrentSongForEveryoneInPartyAsync(Party party, PartyGoer user)
        {
            try
            {
                if (party is null)
                {
                    throw new Exception($"Obtaining a party with ID {party.Id} returned null from the database");
                }

                if (!party.Host.Id.Equals(user.Id, StringComparison.CurrentCultureIgnoreCase) &&
                !party.Attendees.Exists(p => p.Id.Equals(user.Id, StringComparison.CurrentCultureIgnoreCase)))
                {
                    throw new Exception($"A non host or party attendee tried to change the song for the party with ID {party.Id}. Attempted user ID: {user.Id}");
                }

                // Get the current song from the host
                CurrentSongDTO song = await _spotifyHttpClient.GetCurrentSongAsync(user.Id);

                List<Task<bool>> updateSongForPartyTask = new List<Task<bool>>();
                foreach (PartyGoer attendee in party.Attendees)
                {
                    updateSongForPartyTask.Add(_spotifyHttpClient.UpdateSongForPartyGoerAsync(attendee.Id, new List<string> { song.TrackUri }, song.ProgressMs));
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

        public async Task<bool> UpdatePartyPlaylistForEveryoneInPartyAsync(Party party, PartyGoer user)
        {
            try
            {
                if (party is null)
                {
                    throw new Exception($"Obtaining a party with ID {party.Id} returned null from the database");
                }

                if (!party.Host.Id.Equals(user.Id, StringComparison.CurrentCultureIgnoreCase) &&
                !party.Attendees.Exists(p => p.Id.Equals(user.Id, StringComparison.CurrentCultureIgnoreCase)))
                {
                    throw new Exception($"A non host or party attendee tried to change the song for the party with ID {party.Id}. Attempted user ID: {user.Id}");
                }

                List<Task<List<string>>> topTrackUrisTasks = new List<Task<List<string>>>();

                topTrackUrisTasks.Add(_spotifyHttpClient.GetUserTopTrackIdsAsync(user.Id, 5));

                foreach (PartyGoer attendee in party.Attendees)
                {
                    topTrackUrisTasks.Add(_spotifyHttpClient.GetUserTopTrackIdsAsync(attendee.Id, 5));
                }

                var recommendTrackUris = await _spotifyHttpClient.GetRecommendedTrackUrisAsync(user.Id, GetNNumberOfTrackUris(topTrackUrisTasks.SelectMany(p => p.Result).ToList(), 5));

                List<Task<bool>> updateSongForPartyTask = new List<Task<bool>>();
                foreach (PartyGoer attendee in party.Attendees)
                {
                    updateSongForPartyTask.Add(_spotifyHttpClient.UpdateSongForPartyGoerAsync(attendee.Id, recommendTrackUris, 0));
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

        public async Task<List<Song>> CreatePartyPlaylistForEveryoneInPartyAsync(Party party, PartyGoer user)
        {
            try
            {
                if (party is null)
                {
                    throw new Exception($"Obtaining a party with ID {party.Id} returned null from the database");
                }

                if (!party.Host.Id.Equals(user.Id, StringComparison.CurrentCultureIgnoreCase) &&
                !party.Attendees.Exists(p => p.Id.Equals(user.Id, StringComparison.CurrentCultureIgnoreCase)))
                {
                    throw new Exception($"A non host or party attendee tried to change the song for the party with ID {party.Id}. Attempted user ID: {user.Id}");
                }

                List<Task<List<string>>> topTrackUrisTasks = new List<Task<List<string>>>();

                topTrackUrisTasks.Add(_spotifyHttpClient.GetUserTopTrackIdsAsync(user.Id, 5));

                foreach (PartyGoer attendee in party.Attendees)
                {
                    topTrackUrisTasks.Add(_spotifyHttpClient.GetUserTopTrackIdsAsync(attendee.Id, 5));
                }

                List<Song> playlist = await _spotifyHttpClient.GetRecommendedSongsAsync(user.Id, GetNNumberOfTrackUris(topTrackUrisTasks.SelectMany(p => p.Result).ToList(), 5), 5);

                party.CreatePlaylist(new Playlist(playlist, party.Attendees, party.PartyCode));

                return playlist;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<Party> GetPartyAsync(PartyCodeDTO partyCode)
        {
            return await _partyRepository.GetAsync(partyCode);
        }

        public async Task<Party> GetPartyWithHostAsync(PartyGoer host)
        {
            return await _partyRepository.GetPartyWithHostAsync(host);
        }

        public async Task<bool> IsUserPartyingAsync(PartyGoer user)
        {
            return await _partyRepository.IsUserInAPartyAsync(user);
        }

        public async Task<Party> GetPartyWithAttendeeAsync(PartyGoer attendee)
        {
            return await _partyRepository.GetPartyWithAttendeeAsync(attendee);
        }

        public async Task<bool> LeavePartyAsync(PartyGoer attendee)
        {
            // Let's make sure he is part of the party
            if (await _partyRepository.IsUserInAPartyAsync(attendee))
            {
                return await _partyRepository.LeavePartyAsync(attendee);
            }
            else
            {
                return false;
            }
        }

        private List<string> GetNNumberOfTrackUris(List<string> topTrackUris, int selectNTracks)
        {
            return topTrackUris.OrderBy(p => _random.Next()).Take(selectNTracks).ToList();
        }

    }
}
