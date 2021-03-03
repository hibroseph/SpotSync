using SpotSync.Domain.Events;
using SpotSync.Domain.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpotSync.Domain
{
    public class Party : IEquatable<Party>
    {
        private const int LENGTH_OF_PARTY_CODE = 6;

        private PartyGoer _host;
        private Dictionary<string, PartyGoer> _listeners;
        private string _partyCode;
        private List<Track> _queue;
        private List<Track> _history;
        private Track _currentTrack;
        private Stopwatch _trackPositionTime;
        private Timer _nextTrackTimer;
        private List<PartyGoer> _usersThatHaveRequestedSkip;

        public Party(PartyGoer host)
        {
            _host = host;
            _listeners = new Dictionary<string, PartyGoer> { { host.Id, host } };
            _partyCode = GeneratePartyCode();
            _trackPositionTime = new Stopwatch();
            _queue = new List<Track>();
            _history = new List<Track>();
            _nextTrackTimer = new Timer(async (obj) => await NextTrackAsync());
            _usersThatHaveRequestedSkip = new List<PartyGoer>();
        }

        public void EndParty()
        {
            _nextTrackTimer = null;
        }

        public async Task SyncListenerWithSongAsync(PartyGoer listener)
        {
            await DomainEvents.RaiseAsync(new ChangeTrack { Listeners = new List<PartyGoer> { listener }, PartyCode = GetPartyCode(), ProgressMs = GetCurrentPositionInSong(), Track = GetCurrentSong() });
        }

        public async Task AddNewQueueAsync(List<Track> queue)
        {
            _queue = queue;

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

        public async Task RequestSkip(PartyGoer partyGoer)
        {
            _usersThatHaveRequestedSkip.Add(partyGoer);

            if (_usersThatHaveRequestedSkip.Count / _listeners.Count > 0.5)
            {
                await NextTrackAsync();
                _usersThatHaveRequestedSkip.Clear();
            }
        }

        private async Task NextTrackAsync()
        {
            if (_queue.Count > 0)
            {
                _history.Add(_queue.First());

                _queue.RemoveAt(0);
            }

            if (_queue.Count > 0)
            {
                _currentTrack = _queue.First();

                _nextTrackTimer.Change(_currentTrack.Length, Timeout.Infinite);

                _trackPositionTime.Restart();

                _usersThatHaveRequestedSkip.Clear();

                await DomainEvents.RaiseAsync(new ChangeTrack { PartyCode = _partyCode, Listeners = _listeners.Values.ToList(), Track = _currentTrack, ProgressMs = 0 });
            }
            else
            {
                await DomainEvents.RaiseAsync(new PlaylistEnded { PartyCode = _partyCode });
            }
        }

        private async Task StartQueueAsync()
        {
            _currentTrack = _queue.First();

            _nextTrackTimer.Change(_currentTrack.Length, Timeout.Infinite);

            _trackPositionTime.Restart();

            _usersThatHaveRequestedSkip.Clear();

            await DomainEvents.RaiseAsync(new ChangeTrack { PartyCode = _partyCode, Listeners = _listeners.Values.ToList(), Track = _currentTrack, ProgressMs = 0 });
        }

        public string GetPartyCode()
        {
            return _partyCode;
        }

        public bool HasExplicitTracks()
        {
            return _queue.Any(track => track.Explicit == true);
        }

        public async Task TogglePlaybackAsync(PartyGoer partyGoer)
        {
            // Grabbing partier by reference, so any change I make to it will change it in the list
            if (_listeners.TryGetValue(partyGoer.Id, out PartyGoer partier))
            {
                partier.PausedMusic = !partier.PausedMusic;

                await DomainEvents.RaiseAsync(new ToggleMusicState { Listener = partier, State = DeterminePlaybackState(partier.PausedMusic) });
            }
        }

        private PlaybackState DeterminePlaybackState(bool isMusicPaused)
        {
            return isMusicPaused ? PlaybackState.Pause : PlaybackState.Play;
        }

        public void RearrangeTrackInQueue(RearrangeQueueRequest request)
        {
            Track songToBeMoved = _queue[request.OldTrackIndex];
            _queue.RemoveAt(request.OldTrackIndex);
            _queue.Insert(request.NewTrackIndex, songToBeMoved);
        }

        public void AddTrackToQueue(AddSongToQueueRequest request)
        {
            _queue.Add(new Track
            {
                Artist = request.Artist,
                AlbumImageUrl = request.AlbumImageUrl,
                Explicit = request.Explicit,
                Length = request.Length,
                Name = request.Name,
                Uri = request.TrackUri
            });
        }

        public bool IsPartyPlayingMusic()
        {
            return _currentTrack != null;
        }

        public void JoinParty(PartyGoer partyGoer)
        {
            _listeners.TryAdd(partyGoer.Id, partyGoer);
        }

        public void LeaveParty(PartyGoer partyGoer)
        {
            _listeners.Remove(partyGoer.Id);

            if (IsHost(partyGoer))
            {
                _host = _listeners.ElementAt(0).Value;
            }

        }

        public List<Track> GetQueue()
        {
            return _queue;
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
            return hostInQuestion == _host;
        }

        public bool IsListener(PartyGoer listenerInQuestion)
        {
            return _listeners.ContainsKey(listenerInQuestion.Id);
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
            return _currentTrack;
        }

        public int GetListenerCount()
        {
            return _listeners.Count;
        }
    }
}
