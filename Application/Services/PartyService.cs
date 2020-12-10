using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using SpotSync.Domain.Errors;
using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Application.Services
{
    public class PartyService : IPartyService
    {
        private IPartyRepository _partyRepository;
        private IPartyGoerService _partyGoerService;
        private ISpotifyHttpClient _spotifyHttpClient;
        private ILogService _logService;
        private Random _random;

        public PartyService(IPartyRepository partyRepository, ISpotifyHttpClient spotifyHttpClient, ILogService logService, IPartyGoerService partyGoerService)
        {
            _partyRepository = partyRepository;
            _spotifyHttpClient = spotifyHttpClient;
            _partyGoerService = partyGoerService;
            _random = new Random();
            _logService = logService;
        }
        public async Task<bool> AddNewSongToQueue(AddSongToQueueRequest request)
        {
            try
            {
                Party party = await _partyRepository.GetPartyWithCode(request.PartyCode);

                await party.ModifyPlaylistAsync(request);

                return true;
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occured in AddNewSongToQueue");
                return false;
            }
        }

        public async Task<bool> RearrangeQueue(RearrangeQueueRequest request)
        {
            try
            {
                Party party = await _partyRepository.GetPartyWithCode(request.PartyCode);

                await party.ModifyPlaylistAsync(request);

                return true;
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occured in RearrangeQueue");
                return false;
            }

        }

        public async Task<Party> GetPartyWithCodeAsync(string partyCode)
        {
            try
            {
                return await _partyRepository.GetPartyWithCode(partyCode);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in GetPartyWithCodeAsync()");
                return null;
            }
        }

        public async Task SyncUserWithSong(PartyGoer listener)
        {
            Party party = await _partyRepository.GetPartyWithAttendeeAsync(listener);

            await DomainEvents.RaiseAsync(new ChangeSong { Listeners = new List<PartyGoer> { listener }, PartyCode = party.PartyCode, ProgressMs = party.Playlist.CurrentPositionInSong(), Song = party.Playlist.CurrentSong });
            //await _spotifyHttpClient.UpdateSongForPartyGoerAsync(listener.Id, party.Playlist.CurrentSong.TrackUri, party.Playlist.CurrentPositionInSong());
        }

        public async Task<string> StartPartyWithSeedSongsAsync(List<string> seedTrackUris, PartyGoer host)
        {
            Party newParty = new Party(host);

            List<Track> playlistSongs = await _spotifyHttpClient.GetRecommendedSongsAsync(host.Id, seedTrackUris, 0);

            newParty.Playlist = new Playlist(playlistSongs, newParty.Listeners, newParty.PartyCode);

            _partyRepository.CreateParty(newParty);

            await newParty.Playlist.StartAsync();

            return newParty.PartyCode;
        }

        public async Task<string> StartPartyAsync()
        {
            Party party = new Party(await _partyGoerService.GetCurrentPartyGoerAsync());

            _partyRepository.CreateParty(party);

            return party.PartyCode;
        }

        public async Task<bool> IsUserHostingAPartyAsync(PartyGoer host)
        {
            try
            {
                return _partyRepository.IsUserHostingAParty(host);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in IsUserHostiongAPartyAsync");
                return false;
            }
        }

        public async Task<bool> EndPartyAsync(PartyGoer host)
        {
            try
            {
                return await _partyRepository.DeleteAsync(host);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in EndPartyAsync");
                return false;
            }
        }

        public async Task<bool> EndPartyAsync(string partyCode)
        {
            try
            {
                return await _partyRepository.DeleteAsync(partyCode);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in EndPartyAsync");
                return false;
            }
        }

        public async Task<bool> JoinPartyAsync(PartyCodeDTO partyCode, PartyGoer attendee)
        {
            try
            {
                Party party = await _partyRepository.GetAsync(partyCode);

                if (party == null)
                {
                    return false;
                }

                return await JoinPartyAsync(party.Id, attendee);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in JoinPartyAsync");
                return false;
            }
        }
        private async Task<bool> JoinPartyAsync(Guid partyId, PartyGoer attendee)
        {
            try
            {
                Party party = _partyRepository.Get(partyId);

                if (party.Host == null)
                {
                    party.Host = attendee;
                }

                party.JoinParty(attendee);

                _partyRepository.Update(party);

                return true;
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in JoinPartyAsync");
                return false;
            }
        }

        public async Task<string> StartNewPartyAsync(PartyGoer partyHost)
        {
            try
            {
                Party party = new Party(partyHost);

                /*party.CreatePlaylist(new Playlist(new List<Song> { new Song { Length = 4000 }, new Song { Length = 8000 }, new Song { Length = 1000 } }));
                party.StartPlaylist();
                */

                _partyRepository.CreateParty(party);

                return party.PartyCode;
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in StartNewPartyAsync");
                return null;
            }
        }

        public bool NextSong(Track song)
        {
            return true;
        }

        public async Task<ServiceResult<UpdateSongError>> UpdateCurrentSongForEveryoneInPartyAsync(Party party, PartyGoer user)
        {
            try
            {
                if (party is null)
                {
                    throw new Exception($"Obtaining a party with ID {party.Id} returned null from the database");
                }

                if (!party.Host.Id.Equals(user.Id, StringComparison.CurrentCultureIgnoreCase) &&
                !party.Listeners.Exists(p => p.Id.Equals(user.Id, StringComparison.CurrentCultureIgnoreCase)))
                {
                    throw new Exception($"A non host or party attendee tried to change the song for the party with ID {party.Id}. Attempted user ID: {user.Id}");
                }

                // Get the current song from the host
                CurrentSongDTO song = await _spotifyHttpClient.GetCurrentSongAsync(user.Id);

                List<Task<ServiceResult<UpdateSongError>>> updateSongForPartyTask = new List<Task<ServiceResult<UpdateSongError>>>();

                foreach (PartyGoer attendee in party.Listeners)
                {
                    updateSongForPartyTask.Add(_spotifyHttpClient.UpdateSongForPartyGoerAsync(attendee.Id, new List<string> { song.TrackUri }, song.ProgressMs));
                }

                await Task.WhenAll(updateSongForPartyTask);

                ServiceResult<UpdateSongError> errors = new ServiceResult<UpdateSongError>();

                // Verify all the updates worked
                foreach (Task<ServiceResult<UpdateSongError>> task in updateSongForPartyTask)
                {
                    if (task.IsCompletedSuccessfully && !task.Result.Success)
                    {
                        foreach (var error in task.Result.Errors)
                        {
                            errors.AddError(error);
                        }
                    }
                }

                return errors;
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in UpdateCurrentSongForEveryoneInPartyAsync");

                ServiceResult<UpdateSongError> error = new ServiceResult<UpdateSongError>();
                error.AddError(new UpdateSongError("Unable to update everyone's song."));

                return error;
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
                !party.Listeners.Exists(p => p.Id.Equals(user.Id, StringComparison.CurrentCultureIgnoreCase)))
                {
                    throw new Exception($"A non host or party attendee tried to change the song for the party with ID {party.Id}. Attempted user ID: {user.Id}");
                }

                List<Task<List<string>>> topTrackUrisTasks = new List<Task<List<string>>>();

                topTrackUrisTasks.Add(_spotifyHttpClient.GetUserTopTrackIdsAsync(user.Id, 5));

                foreach (PartyGoer attendee in party.Listeners)
                {
                    topTrackUrisTasks.Add(_spotifyHttpClient.GetUserTopTrackIdsAsync(attendee.Id, 5));
                }

                var recommendTrackUris = await _spotifyHttpClient.GetRecommendedTrackUrisAsync(user.Id, GetNNumberOfTrackUris(topTrackUrisTasks.SelectMany(p => p.Result).ToList(), 5));

                List<Task<ServiceResult<UpdateSongError>>> updateSongForPartyTask = new List<Task<ServiceResult<UpdateSongError>>>();
                foreach (PartyGoer attendee in party.Listeners)
                {
                    updateSongForPartyTask.Add(_spotifyHttpClient.UpdateSongForPartyGoerAsync(attendee.Id, recommendTrackUris, 0));
                }

                await Task.WhenAll(updateSongForPartyTask);

                // Verify all the updates worked
                foreach (Task<ServiceResult<UpdateSongError>> task in updateSongForPartyTask)
                {
                    if (!task.Result.Success)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in UpdatePartyPlaylistForEveryoneInPartyAsync");
                return false;
            }

        }

        public async Task<List<Track>> CreatePartyPlaylistForEveryoneInPartyAsync(Party party, PartyGoer user)
        {
            try
            {
                if (party is null)
                {
                    throw new Exception($"Obtaining a party with ID {party.Id} returned null from the database");
                }

                if (!party.Host.Id.Equals(user.Id, StringComparison.CurrentCultureIgnoreCase) &&
                !party.Listeners.Exists(p => p.Id.Equals(user.Id, StringComparison.CurrentCultureIgnoreCase)))
                {
                    throw new Exception($"A non host or party attendee tried to change the song for the party with ID {party.Id}. Attempted user ID: {user.Id}");
                }

                List<Task<List<string>>> topTrackUrisTasks = new List<Task<List<string>>>();

                topTrackUrisTasks.Add(_spotifyHttpClient.GetUserTopTrackIdsAsync(user.Id, 5));

                foreach (PartyGoer attendee in party.Listeners)
                {
                    topTrackUrisTasks.Add(_spotifyHttpClient.GetUserTopTrackIdsAsync(attendee.Id, 5));
                }

                List<Track> playlist = await _spotifyHttpClient.GetRecommendedSongsAsync(user.Id, GetNNumberOfTrackUris(topTrackUrisTasks.SelectMany(p => p.Result).ToList(), 5), 5);

                List<PartyGoer> partyGoersWithHost = party.Listeners.Select(p => p).ToList();
                partyGoersWithHost.Add(new PartyGoer(user.Id));

                if (party.Playlist != null)
                {
                    await party.DeletePlaylistAsync();
                }

                party.Playlist = new Playlist(playlist, partyGoersWithHost, party.PartyCode);
                await party.Playlist.StartAsync();
                //party.CreatePlaylist(new Playlist(playlist, partyGoersWithHost, party.PartyCode));

                return playlist;
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in CreatePartyPlaylistForEveryoneInPartyAsync");
                return null;
            }

        }

        public async Task<Party> GetPartyAsync(PartyCodeDTO partyCode)
        {
            try
            {
                return await _partyRepository.GetAsync(partyCode);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in GetPartyAsync");
                return null;
            }
        }

        public async Task<Party> GetPartyWithHostAsync(PartyGoer host)
        {
            try
            {
                return await _partyRepository.GetPartyWithHostAsync(host);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in GetPartywithHostAsync");
                return null;
            }
        }

        public async Task<bool> IsUserPartyingAsync(PartyGoer user)
        {
            try
            {
                return await _partyRepository.IsUserInAPartyAsync(user);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in IsUserPartyingAsync");
                return false;
            }
        }

        public async Task<Party> GetPartyWithAttendeeAsync(PartyGoer attendee)
        {
            try
            {
                return await _partyRepository.GetPartyWithAttendeeAsync(attendee);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in GetPartyWithAttendeeAsync");
                return null;
            }
        }

        public async Task<bool> LeavePartyAsync(PartyGoer attendee)
        {
            try
            {
                if (await IsUserPartyingAsync(attendee))
                {
                    _partyRepository.LeaveParty(attendee);
                }

                if (await IsUserHostingAPartyAsync(attendee))
                {
                    await _partyRepository.RemoveHostFromPartyAsync(attendee);
                }

                return true;

            }
            catch (PartyGoerWasInMultiplePartiesException ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in LeavePartyAsync");
                return true;
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in LeavePartyAsync");
                return false;
            }
        }

        private List<string> GetNNumberOfTrackUris(List<string> topTrackUris, int selectNTracks)
        {
            return topTrackUris.OrderBy(p => _random.Next()).Take(selectNTracks).ToList();
        }

        public async Task<List<Party>> GetTopParties(int count)
        {
            // Currently this gets the parties with the most listeners
            return await _partyRepository.GetPartiesWithMostListenersAsync(count);
        }
    }
}
