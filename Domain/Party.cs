﻿using SpotSync.Domain.Events;
using SpotSync.Domain.Types;
using System;
using System.Collections.Concurrent;
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
        private Queue _queue;
        private List<Track> _history;
        private TrackWithFeelings _currentTrack;
        private Stopwatch _trackPositionTime;
        private Timer _nextTrackTimer;

        public Party(PartyGoer host)
        {
            _host = host;
            _listeners = new Dictionary<string, PartyGoer> { { host.Id, host } };
            _partyCode = GeneratePartyCode();
            _trackPositionTime = new Stopwatch();
            _queue = new Queue();
            _history = new List<Track>();
            _nextTrackTimer = new Timer(async (obj) => await NextTrackAsync());
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
                LikedSongs = _queue.GetLikedTracksUris()
            };
        }

        public Task UserLikesTrackAsync(PartyGoer partyGoer, string trackUri)
        {
            _queue.UserLikesTrack(partyGoer, trackUri);

            return Task.CompletedTask;
        }

        public async Task UserDislikesTrackAsync(PartyGoer partyGoer, string trackUri)
        {
            _queue.UserDislikesTrack(partyGoer, trackUri);

            if (IsNowPlayingTrack(trackUri))
            {
                await RequestSkipAsync(partyGoer);
            }
        }

        private bool IsNowPlayingTrack(string trackUri)
        {
            return _currentTrack.GetTrackWithoutFeelings().Uri.Equals(trackUri, StringComparison.OrdinalIgnoreCase);
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
                await DomainEvents.RaiseAsync(new QueueEnded { PartyCode = _partyCode, LikedTracksUris = GetLikedTracksUris(5) });
            }
        }

        public List<string> GetLikedTracksUris(int amount)
        {
            if (_queue.TracksAreLiked())
            {
                return _queue.GetRandomLikedTrackUris(5);
            }
            else
            {
                return _history.GetRandomNItems(amount).Select(track => track.Uri).ToList();
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

        public async Task AddTrackToQueueAsync(AddSongToQueueRequest request)
        {
            _queue.QueueTrack(new Track
            {
                Artist = request.Artist,
                AlbumImageUrl = request.AlbumImageUrl,
                Explicit = request.Explicit,
                Length = request.Length,
                Name = request.Name,
                Uri = request.TrackUri
            });


            // check to see if music is playing, if not, lets start it
            if (_currentTrack == null)
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
            _listeners.TryAdd(partyGoer.Id, partyGoer);

            if (_listeners.Count == 1)
            {
                _host = partyGoer;
            }
        }

        public void LeaveParty(PartyGoer partyGoer)
        {
            _listeners.Remove(partyGoer.Id);

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
            return _currentTrack?.GetTrackWithoutFeelings();
        }

        public int GetListenerCount()
        {
            return _listeners.Count;
        }
    }
}
