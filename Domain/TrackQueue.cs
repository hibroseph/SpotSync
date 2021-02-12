using SpotSync.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain
{
    public class TrackQueue : ITrackQueue
    {
        private List<Track> _tracks;

        public int Count => _tracks.Count;

        public Track First => throw new NotImplementedException();

        public TrackQueue()
        {
            _tracks = new List<Track>();
        }


        public TrackQueue(List<Track> tracks)
        {
            _tracks = tracks;
        }

        public bool HasExplicitSongs()
        {
            return _tracks.Any(p => p.Explicit);
        }

        public Task RearrangeQueueAsync(RearrangeQueueRequest request)
        {
            throw new NotImplementedException();
        }

        public Task AddSongToQueueAsync(AddSongToQueueRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
