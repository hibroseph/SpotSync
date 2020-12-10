using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpotSync.Domain
{
    public class Playlist
    {
        public Track CurrentSong { get; private set; }
        public List<QueuedTrack> Queue { get; private set; }
        public Queue<Track> History { get; private set; }
        private Timer _nextSongTimer;
        private List<PartyGoer> _listeners;
        private string _partyCode;
        private Stopwatch _songPositionTime;

        public Playlist(List<PartyGoer> listeners, string partyCode)
        {
            Queue = new List<QueuedTrack>();
            History = new Queue<Track>();
            _songPositionTime = new Stopwatch();
            _partyCode = partyCode;
            _listeners = listeners;
            CurrentSong = null;
        }
        public Playlist(List<Track> songs, List<PartyGoer> listeners, string partyCode)
        {
            Queue = songs.Select(track =>
            {
                return new QueuedTrack
                {
                    AlbumImageUrl = track.AlbumImageUrl,
                    Artist = track.Artist,
                    Explicit = track.Explicit,
                    Length = track.Length,
                    Name = track.Name,
                    Uri = track.Uri,
                    AddedBy = null
                };
            }).ToList();
            History = new Queue<Track>();
            _listeners = listeners;
            _partyCode = partyCode;
            _songPositionTime = new Stopwatch();
            CurrentSong = null;
        }

        public Playlist(List<Track> songs, List<PartyGoer> listeners, string partyCode, Queue<Track> existingHistory)
        {
            Queue = songs.Select(track =>
            {
                return new QueuedTrack
                {
                    AlbumImageUrl = track.AlbumImageUrl,
                    Artist = track.Artist,
                    Explicit = track.Explicit,
                    Length = track.Length,
                    Name = track.Name,
                    Uri = track.Uri,
                    AddedBy = null
                };
            }).ToList();

            History = existingHistory;
            _listeners = listeners;
            _partyCode = partyCode;
            _songPositionTime = new Stopwatch();
            CurrentSong = null;
        }

        public void RemoveListener(PartyGoer listener)
        {
            _listeners.RemoveAll(p => p.Id.Equals(listener.Id, StringComparison.OrdinalIgnoreCase));
        }

        public async Task DeleteAsync()
        {
            _songPositionTime?.Stop();
            _songPositionTime = null;
            _nextSongTimer?.DisposeAsync();
            _nextSongTimer = null;
            CurrentSong = null;
            Queue = null;
            History = null;
            _partyCode = null;
        }

        public async Task StartAsync()
        {
            if (Queue is null || Queue.Count == 0)
                throw new ArgumentNullException("Queue cannot be null");

            if (CurrentSong is null)
            {
                CurrentSong = Queue.First();
                await DomainEvents.RaiseAsync(new ChangeSong { PartyCode = _partyCode, Listeners = _listeners, Song = CurrentSong, ProgressMs = 0 });
                _nextSongTimer = new Timer(async state => await NextSongAsync(), null, CurrentSong.Length, Timeout.Infinite);
                _songPositionTime.Start();
            }
        }

        public Task ModifyQueueAsync(RearrangeQueueRequest request)
        {
            QueuedTrack songToBeMoved = Queue[request.OldTrackIndex];
            Queue.RemoveAt(request.OldTrackIndex);
            Queue.Insert(request.NewTrackIndex, songToBeMoved);

            return Task.CompletedTask;
        }

        public Task AddSongToQueueAsync(AddSongToQueueRequest request)
        {
            if (request.IndexToInsertSongAt >= 0)
            {
                Queue.Insert(request.IndexToInsertSongAt, new QueuedTrack
                {
                    AlbumImageUrl = request.AlbumImageUrl,
                    Artist = request.Artist,
                    Length = request.Length,
                    Name = request.Name,
                    Uri = request.TrackUri,
                    AddedBy = request.AddedBy,
                    Explicit = request.Explicit
                });
            }
            else
            {
                Queue.Add(new QueuedTrack
                {
                    AlbumImageUrl = request.AlbumImageUrl,
                    Artist = request.Artist,
                    Length = request.Length,
                    Name = request.Name,
                    Uri = request.TrackUri,
                    AddedBy = request.AddedBy,
                    Explicit = request.Explicit
                });
            }

            return Task.CompletedTask;
        }

        public async Task NextSongAsync()
        {
            History.Enqueue(Queue.First());
            Queue.RemoveAt(0);
            if (Queue.Count > 0)
            {
                CurrentSong = Queue.First();
                _nextSongTimer.Change(CurrentSong.Length, Timeout.Infinite);
                _songPositionTime.Restart();
                // update song for all those in party
                await DomainEvents.RaiseAsync(new ChangeSong { PartyCode = _partyCode, Listeners = _listeners, Song = CurrentSong, ProgressMs = 0 });
            }
            else
            {
                CurrentSong = null;
                await DomainEvents.RaiseAsync(new PlaylistEnded { PartyCode = _partyCode });
                // lets generate 15 more songs
            }
        }

        public int CurrentPositionInSong()
        {
            return (int)_songPositionTime.ElapsedMilliseconds;
        }
    }
}
