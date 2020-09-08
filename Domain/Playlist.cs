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
        public Queue<Song> Queue { get; private set; }
        public Queue<Song> History { get; private set; }
        private Timer _timer;
        private List<PartyGoer> _listeners;
        private string _partyCode;
        private Stopwatch _stopWatch;


        public Playlist(List<Song> songs, List<PartyGoer> listeners, string partyCode)
        {
            Queue = new Queue<Song>(songs);
            History = new Queue<Song>();
            _listeners = listeners;
            _partyCode = partyCode;
            _stopWatch = new Stopwatch();
            CurrentSong = null;
        }

        public void Start()
        {
            if (Queue is null || Queue.Count == 0)
                throw new ArgumentNullException("Queue cannot be null");

            if (CurrentSong is null)
            {
                CurrentSong = Queue.First();
                DomainEvents.Raise(new ChangeSong { PartyCode = _partyCode, Listeners = _listeners, Song = CurrentSong, ProgressMs = 0 });
                _timer = new Timer(state => NextSong(), null, CurrentSong.Length, Timeout.Infinite);
                _stopWatch.Start();
            }
        }

        public void NextSong()
        {
            History.Enqueue(Queue.Dequeue());
            if (Queue.Count > 0)
            {
                CurrentSong = Queue.First();
                _timer.Change(CurrentSong.Length, Timeout.Infinite);
                _stopWatch.Restart();
                // update song for all those in party
                DomainEvents.Raise(new ChangeSong { PartyCode = _partyCode, Listeners = _listeners, Song = CurrentSong, ProgressMs = 0 });
            }
            else
            {
                throw new ArgumentNullException("Queue has no more songs");
            }
        }

        public int CurrentPositionInSong()
        {
            return (int)_stopWatch.ElapsedMilliseconds;
        }
    }
}
