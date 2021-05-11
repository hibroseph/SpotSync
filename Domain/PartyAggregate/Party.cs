using SpotSync.Domain.Contracts.SpotibroModels;
using SpotSync.Domain.Contracts.SpotifyApi.Models;
using SpotSync.Domain.Events;
using SpotSync.Domain.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SpotibroModels = SpotSync.Domain.Contracts.SpotibroModels;

namespace SpotSync.Domain.PartyAggregate
{
    public class Party : IEquatable<Party>
    {
        private const int LENGTH_OF_PARTY_CODE = 6;

        private PartyGoer _host;
        private Dictionary<string, PartyGoer> _listeners;
        private string _partyCode;
        private Queue _queue;
        private List<Track> _history;
        private TrackWithFeelings _currentTrack;
        private Stopwatch _trackPositionTime;
        private Timer _nextTrackTimer;
        private List<PartyGoer> _usersThatDislikeCurrentSong;
        private ContributionManager _contributionManager;

        public Party(PartyGoer host)
        {
            _host = host;
            _listeners = new Dictionary<string, PartyGoer> { { host.GetId(), host } };
            _partyCode = GeneratePartyCode();
            _trackPositionTime = new Stopwatch();
            _queue = new Queue();
            _history = new List<Track>();
            _nextTrackTimer = new Timer(async (obj) => await NextTrackAsync());
            _usersThatDislikeCurrentSong = new List<PartyGoer>();
            _contributionManager = new ContributionManager();
        }

        public async Task UpdateContributionsAsync(List<Contribution> contributionsToAdd, List<Contribution> contributionsToRemove)
        {
            _contributionManager.RemoveContributions(contributionsToRemove);
            _contributionManager.AddContributions(contributionsToAdd);

            // if no music is playing
            if (!HasPartyStarted())
            {
                // start party with contributions
                var seeds = _contributionManager.GetContributionSeeds(GetListeners());
                await DomainEvents.RaiseAsync(new QueueEnded
                {
                    PartyCode = _partyCode,
                    SeedTracksUris = seeds.ContainsKey(ContributionType.Track) ? seeds[ContributionType.Track] : new List<string>(),
                    SeedArtistUris = seeds.ContainsKey(ContributionType.Artist) ? seeds[ContributionType.Artist] : new List<string>()
                });
            }
        }

        public Task<List<PartierContribution>> GetUserContributionsAsync(PartyGoer partier)
        {
            return Task.FromResult(_contributionManager.GetContributions(partier));
        }

        public void RemoveContribution(PartyGoer partier, Guid contributionId)
        {
            _contributionManager.RemoveContribution(partier, contributionId);
        }

        private bool HasPartyStarted()
        {
            return _currentTrack != null;
        }

        public Dictionary<string, int> GetTrackVotes()
        {
            return _queue.GetTrackVoting();
        }

        public void NukeQueue(PartyGoer partier)
        {
            if (IsHost(partier))
            {
                _queue.Nuke();
            }
        }

        public LikesDislikes GetUsersLikesDislikes(PartyGoer partyGoer)
        {
            return _queue.GetUsersTrackFeelings(partyGoer);
        }

        public PartyGoer GetHost()
        {
            return _host;
        }

        public PartyDiagnostics GetDiagnostics()
        {
            return new PartyDiagnostics
            {
                Host = _host,
                Listeners = _listeners.Select(pair => pair.Value).ToList(),
                PartyCode = _partyCode,
                Queue = _queue.GetAllTracks(),
                History = _history,
                CurrentTrack = _currentTrack.GetTrackWithoutFeelings(),
                LikedSongs = _queue.GetLikedTrackUris()
            };
        }

        public async Task UserLikesTrackAsync(PartyGoer partyGoer, string trackUri)
        {
            await _queue.UserLikesTrackAsync(partyGoer, trackUri, _partyCode);
        }

        public async Task UserDislikesTrackAsync(PartyGoer partyGoer, string trackUri)
        {
            if (IsNowPlayingTrack(trackUri))
            {
                if (IsHost(partyGoer))
                {
                    await RequestSkipAsync(partyGoer);
                    return;
                }

                await UserDislikesCurrentSongAsync(partyGoer);
                return;
            }


            await _queue.UserDislikesTrackAsync(partyGoer, trackUri, _listeners.Count(), _partyCode);
        }

        private async Task UserDislikesCurrentSongAsync(PartyGoer partyGoer)
        {
            _usersThatDislikeCurrentSong.Add(partyGoer);

            if (_usersThatDislikeCurrentSong.Count >= 0.5 * _listeners.Count)
            {
                _usersThatDislikeCurrentSong.Clear();
                await NextTrackAsync();
            }
        }

        private bool IsNowPlayingTrack(string trackUri)
        {
            return _currentTrack.GetTrackWithoutFeelings().Id.Equals(trackUri, StringComparison.OrdinalIgnoreCase);
        }

        public void EndParty()
        {
            _nextTrackTimer = null;
        }

        public async Task SyncListenerWithSongAsync(PartyGoer listener)
        {
            await DomainEvents.RaiseAsync(new ChangeTrack { Listeners = new List<PartyGoer> { listener }, PartyCode = GetPartyCode(), ProgressMs = GetCurrentPositionInSong(), Track = GetCurrentSong() });
        }

        public async Task AddNewQueueAsync(List<Track> tracks)
        {
            _queue = new Queue(tracks);

            await StartQueueAsync();
        }

        public Task UpdateCurrentSongForPartyAsync()
        {
            throw new NotImplementedException();
        }

        public List<PartyGoer> GetListeners()
        {
            return _listeners.Select(p => p.Value).ToList();
        }

        public async Task RequestSkipAsync(PartyGoer partyGoer)
        {
            if (IsHost(partyGoer))
            {
                await NextTrackAsync();
            }
        }

        private void UpdateTimerWithNewTrack()
        {
            _nextTrackTimer.Change(_currentTrack.GetTrackWithoutFeelings().Length, Timeout.Infinite);

            _trackPositionTime.Restart();

        }

        private void AddCurrentTrackToHistory()
        {
            if (_currentTrack != null)
            {
                _history.Add(_currentTrack.GetTrackWithoutFeelings());
            }
        }

        private async Task NextTrackAsync()
        {
            AddCurrentTrackToHistory();

            if (_queue.HasNextTrack())
            {
                _currentTrack = _queue.NextTrack();

                UpdateTimerWithNewTrack();

                await DomainEvents.RaiseAsync(new ChangeTrack { PartyCode = _partyCode, Listeners = _listeners.Values.ToList(), Track = _currentTrack.GetTrackWithoutFeelings(), ProgressMs = 0 });
            }
            else
            {
                _currentTrack = null;
                var seeds = GetSeedUris(5);
                await DomainEvents.RaiseAsync(new QueueEnded { PartyCode = _partyCode, SeedTracksUris = seeds.Item1, SeedArtistUris = seeds.Item2 });
            }
        }

        public Tuple<List<string>, List<string>> GetSeedUris(int amount)
        {
            Dictionary<ContributionType, List<string>> seeds = _contributionManager.GetContributionSeeds(GetListeners());

            if (seeds.Count > 0)
            {
                return new Tuple<List<string>, List<string>>(seeds.ContainsKey(ContributionType.Track) ? seeds[ContributionType.Track] : new List<string>(), seeds.ContainsKey(ContributionType.Artist) ? seeds[ContributionType.Artist] : new List<string>());
            }

            if (_queue.TracksAreLiked())
            {
                return _queue.GetRandomLikedTrackUris(5);
            }
            else
            {
                return new Tuple<List<string>, List<string>>(_history.GetRandomNItems(amount).Select(track => track.Id).ToList(), new List<string>());
            }
        }

        private async Task StartQueueAsync()
        {
            _currentTrack = _queue.NextTrack();

            UpdateTimerWithNewTrack();

            await DomainEvents.RaiseAsync(new ChangeTrack { PartyCode = _partyCode, Listeners = _listeners.Values.ToList(), Track = _currentTrack.GetTrackWithoutFeelings(), ProgressMs = 0 });
        }

        public string GetPartyCode()
        {
            return _partyCode;
        }

        public bool HasExplicitTracks()
        {
            return _queue.HasExplicitTracks();
        }

        public async Task TogglePlaybackAsync(PartyGoer partyGoer)
        {
            // Grabbing partier by reference, so any change I make to it will change it in the list
            if (_listeners.TryGetValue(partyGoer.GetId(), out PartyGoer partier))
            {
                partier.ToggleMusicPlaybackState();

                await DomainEvents.RaiseAsync(new ToggleMusicState { Listener = partier, State = DeterminePlaybackState(partier.IsMusicPaused()) });
            }
        }

        private string DetermineTrackIdExtension(string trackUri)
        {
            return $"+{_queue.GetTrackRepeatNumber(trackUri)}";
        }

        private PlaybackState DeterminePlaybackState(bool isMusicPaused)
        {
            return isMusicPaused ? PlaybackState.Pause : PlaybackState.Play;
        }

        public async Task AddTrackToQueueAsync(AddSongToQueueCommand request)
        {
            Track track = new Track
            {
                Artists = request.Artists,
                AlbumImageUrl = request.AlbumImageUrl,
                Explicit = request.Explicit,
                Length = request.Length,
                Name = request.Name,
                Id = request.TrackUri + DetermineTrackIdExtension(request.TrackUri)
            };

            _queue.QueueTrack(track);


            // check to see if music is playing, if not, lets start it
            if (_currentTrack == null)
            {
                await StartQueueAsync();
            }
        }

        public async Task AddTracksRandomlyToQueueAsync(List<SpotibroModels.Track> playlistItems)
        {
            await _queue.AddTracksRandomlyToQueueAsync(playlistItems);

            if (_currentTrack == null && _queue.SongsExistInQueue())
            {
                await StartQueueAsync();
            }
        }

        public bool IsPartyPlayingMusic()
        {
            return _currentTrack != null;
        }

        public void JoinParty(PartyGoer partyGoer)
        {
            _listeners.TryAdd(partyGoer.GetId(), partyGoer);

            if (_listeners.Count == 1)
            {
                _host = partyGoer;
            }
        }

        public void LeaveParty(PartyGoer partyGoer)
        {
            _listeners.Remove(partyGoer.GetId());

            if (IsHost(partyGoer))
            {
                if (_listeners.Count > 0)
                {
                    _host = _listeners.First().Value;
                }
                else
                {
                    _host = null;
                }
            }

        }

        public List<Track> GetQueue()
        {
            return _queue.GetAllTracks();
        }

        public List<Track> GetHistory()
        {
            return _history;
        }

        private static string GeneratePartyCode()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            // generate 6 random characters
            return new string(Enumerable.Repeat(chars, LENGTH_OF_PARTY_CODE).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public bool IsHost(PartyGoer hostInQuestion)
        {
            return hostInQuestion.Equals(_host);
        }

        public bool IsListener(PartyGoer listenerInQuestion)
        {
            return _listeners.ContainsKey(listenerInQuestion.GetId());
        }
        public bool Equals([AllowNull] Party other)
        {
            return _partyCode.Equals(other.GetPartyCode(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetCurrentPositionInSong()
        {
            return (int)_trackPositionTime.ElapsedMilliseconds;
        }

        public Track GetCurrentSong()
        {
            return _currentTrack?.GetTrackWithoutFeelings();
        }

        public int GetListenerCount()
        {
            return _listeners.Count;
        }
    }
}
