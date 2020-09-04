using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpotSync.Domain
{
    public class Playlist
    {
        private Queue<Song> _queue;
        private Queue<Song> _history;
        private Song _currentSong;
        private DateTime _timeSongStarted;
        private Timer _timer;
        private List<PartyGoer> _listeners;

        public Playlist(List<Song> songs, List<PartyGoer> listeners)
        {
            _queue = new Queue<Song>(songs);
            _history = new Queue<Song>();
            _listeners = listeners;
        }

        public void Start()
        {

            if (_queue is null)
                throw new ArgumentNullException("Queue cannot be null");

            if (_currentSong is null)
            {
                _currentSong = _queue.First();
                _timeSongStarted = DateTime.Now;
                DomainEvents.Raise(new ChangeSong { Listeners = _listeners, Song = _currentSong, ProgressMs = 0 });
                _timer = new Timer(state => NextSong(), null, _currentSong.Length, Timeout.Infinite);
            }
        }

        public void NextSong()
        {
            _history.Enqueue(_queue.Dequeue());
            if (_queue.Count > 0)
            {
                _currentSong = _queue.First();
                _timer.Change(_currentSong.Length, Timeout.Infinite);
                _timeSongStarted = DateTime.Now;

                // update song for all those in party
                DomainEvents.Raise(new ChangeSong { Listeners = _listeners, Song = _currentSong, ProgressMs = 0 });
            }
        }
    }
}
