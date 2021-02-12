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

        public async Task TogglePlaybackStateAsync(string partyCode, PartyGoer partyGoer)
        {
            try
            {
                Party party = await _partyRepository.GetPartyWithCodeAsync(partyCode);

                await party.TogglePlaybackAsync(partyGoer);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in TogglePlaybackStateAsync()");
            }
        }

        public async Task UserWantsToSkipSong(PartyGoer partyGoer, string partyCode)
        {
            try
            {
                Party party = await _partyRepository.GetPartyWithCodeAsync(partyCode);

                await party.RequestSkip(partyGoer);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in UserWantsToSkipSong()");
            }
        }

        public async Task<bool> AddNewSongToQueue(AddSongToQueueRequest request)
        {
            try
            {
                Party party = await _partyRepository.GetPartyWithCodeAsync(request.PartyCode);

                party.AddTrackToQueue(request);

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
                Party party = await _partyRepository.GetPartyWithCodeAsync(request.PartyCode);

                party.RearrangeTrackInQueue(request);

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
                return await _partyRepository.GetPartyWithCodeAsync(partyCode);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in GetPartyWithCodeAsync()");
                return null;
            }
        }

        public async Task SyncUserWithSongAsync(PartyGoer listener)
        {
            Party party = await _partyRepository.GetPartyWithAttendeeAsync(listener);

            if (party != null && party.IsPartyPlayingMusic())
            {
                await DomainEvents.RaiseAsync(new ChangeTrack { Listeners = new List<PartyGoer> { listener }, PartyCode = party.GetPartyCode(), ProgressMs = party.GetCurrentPositionInSong(), Track = party.GetCurrentSong() });
            }
        }

        public async Task<string> StartPartyWithSeedSongsAsync(List<string> seedTrackUris, PartyGoer host)
        {
            Party newParty = new Party(host);

            List<Track> playlistSongs = await _spotifyHttpClient.GetRecommendedSongsAsync(host.Id, seedTrackUris, 0);

            await newParty.AddNewQueueAsync(playlistSongs);

            _partyRepository.CreateParty(newParty);

            return newParty.GetPartyCode();
        }

        public async Task<string> StartPartyAsync()
        {
            Party party = new Party(await _partyGoerService.GetCurrentPartyGoerAsync());

            _partyRepository.CreateParty(party);

            return party.GetPartyCode();
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
                Party party = await _partyRepository.GetPartyWithHostAsync(host);

                if (party != null)
                {
                    return await _partyRepository.DeleteAsync(party.GetPartyCode());
                }

                return false;
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

                return await JoinPartyAsync(partyCode, attendee);
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

                return party.GetPartyCode();
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

        public async Task<Domain.Errors.ServiceResult<UpdateSongError>> UpdateCurrentSongForEveryoneInPartyAsync(Party party, PartyGoer user)
        {
            try
            {
                if (party is null)
                {
                    throw new Exception($"Obtaining a party with code {party.GetPartyCode()} returned null from the database");
                }
                // Get the current song from the host
                CurrentSongDTO song = await _spotifyHttpClient.GetCurrentSongAsync(user.Id);

                List<Task<Domain.Errors.ServiceResult<UpdateSongError>>> updateSongForPartyTask = new List<Task<Domain.Errors.ServiceResult<UpdateSongError>>>();


                foreach (PartyGoer attendee in party.GetListeners())
                {
                    updateSongForPartyTask.Add(_spotifyHttpClient.UpdateSongForPartyGoerAsync(attendee, new List<string> { song.TrackUri }, song.ProgressMs));
                }

                await Task.WhenAll(updateSongForPartyTask);

                Domain.Errors.ServiceResult<UpdateSongError> errors = new Domain.Errors.ServiceResult<UpdateSongError>();

                // Verify all the updates worked
                foreach (Task<Domain.Errors.ServiceResult<UpdateSongError>> task in updateSongForPartyTask)
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

                Domain.Errors.ServiceResult<UpdateSongError> error = new Domain.Errors.ServiceResult<UpdateSongError>();
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
                    throw new Exception($"Obtaining a party with code {party.GetPartyCode()} returned null from the database");
                }

                List<Task<List<string>>> topTrackUrisTasks = new List<Task<List<string>>>();

                topTrackUrisTasks.Add(_spotifyHttpClient.GetUserTopTrackIdsAsync(user.Id, 5));

                foreach (PartyGoer attendee in party.GetListeners())
                {
                    topTrackUrisTasks.Add(_spotifyHttpClient.GetUserTopTrackIdsAsync(attendee.Id, 5));
                }

                var recommendTrackUris = await _spotifyHttpClient.GetRecommendedTrackUrisAsync(user.Id, GetNNumberOfTrackUris(topTrackUrisTasks.SelectMany(p => p.Result).ToList(), 5));

                List<Task<Domain.Errors.ServiceResult<UpdateSongError>>> updateSongForPartyTask = new List<Task<Domain.Errors.ServiceResult<UpdateSongError>>>();
                foreach (PartyGoer attendee in party.GetListeners())
                {
                    updateSongForPartyTask.Add(_spotifyHttpClient.UpdateSongForPartyGoerAsync(attendee, recommendTrackUris, 0));
                }

                await Task.WhenAll(updateSongForPartyTask);

                // Verify all the updates worked
                foreach (Task<Domain.Errors.ServiceResult<UpdateSongError>> task in updateSongForPartyTask)
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
                    throw new Exception($"Obtaining a party with ID {party.GetPartyCode()} returned null from the database");
                }

                List<Task<List<string>>> topTrackUrisTasks = new List<Task<List<string>>>();

                topTrackUrisTasks.Add(_spotifyHttpClient.GetUserTopTrackIdsAsync(user.Id, 5));

                foreach (PartyGoer attendee in party.GetListeners())
                {
                    topTrackUrisTasks.Add(_spotifyHttpClient.GetUserTopTrackIdsAsync(attendee.Id, 5));
                }

                List<Track> playlist = await _spotifyHttpClient.GetRecommendedSongsAsync(user.Id, GetNNumberOfTrackUris(topTrackUrisTasks.SelectMany(p => p.Result).ToList(), 5), 5);

                List<PartyGoer> partyGoersWithHost = party.GetListeners().Select(p => p).ToList();

                partyGoersWithHost.Add(new PartyGoer(user.Id));

                await party.AddNewQueueAsync(playlist);

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
