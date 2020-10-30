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
        public Song CurrentSong { get; private set; }
        public List<Song> Queue { get; private set; }
        public Queue<Song> History { get; private set; }
        private Timer _nextSongTimer;
        private List<PartyGoer> _listeners;
        private string _partyCode;
        private Stopwatch _songPositionTime;

        public Playlist(List<Song> songs, List<PartyGoer> listeners, string partyCode)
        {
            Queue = new List<Song>(songs);
            History = new Queue<Song>();
            _listeners = listeners;
            _partyCode = partyCode;
            _songPositionTime = new Stopwatch();
            CurrentSong = null;
        }

        public Playlist(List<Song> songs, List<PartyGoer> listeners, string partyCode, Queue<Song> existingHistory)
        {
            Queue = new List<Song>(songs);
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
            _songPositionTime.Stop();
            _songPositionTime = null;
            await _nextSongTimer.DisposeAsync();
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
            Song songToBeMoved = Queue[request.OldTrackIndex];
            Queue.RemoveAt(request.OldTrackIndex);
            Queue.Insert(request.NewTrackIndex, songToBeMoved);

            return Task.CompletedTask;
        }

        public Task AddSongToQueueAsync(AddSongToQueueRequest request)
        {
            Queue.Insert(request.IndexToInsertSongAt, new Song
            {
                AlbumImageUrl = request.AlbumImageUrl,
                Artist = request.Artist,
                Length = request.Length,
                Title = request.Title,
                TrackUri = request.TrackUri
            });

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
