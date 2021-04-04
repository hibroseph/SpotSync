using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain
{
    public class Queue
    {
        private List<TrackWithFeelings> _tracks;
        private UsersLikesDislikes _usersLikesDislikes;

        public Queue(List<Track> tracks)
        {
            _tracks = tracks.Select(p => new TrackWithFeelings(p)).ToList();
            _tracks.Sort(new ReorderQueueComparer());
            _usersLikesDislikes = new UsersLikesDislikes();
        }

        public Queue()
        {
            _tracks = new List<TrackWithFeelings>();
            _usersLikesDislikes = new UsersLikesDislikes();
        }

        public List<Track> GetAllTracks()
        {
            return _tracks.Select(p => p.GetTrackWithoutFeelings()).ToList();
        }

        public TrackWithFeelings NextTrack()
        {
            if (HasNextTrack())
            {
                TrackWithFeelings trackToReturn = _tracks.First();

                _tracks.RemoveAt(0);

                return trackToReturn;
            }

            return null;
        }

        public bool TracksAreLiked()
        {
            foreach (TrackWithFeelings track in _tracks)
            {
                if (track.DoUsersLikeSong())
                {
                    return true;
                }
            }

            return false;
        }

        public List<string> GetRandomLikedTrackUris(int amount)
        {
            return _tracks.GetRandomNItems(5).Select(track => track.GetTrackWithoutFeelings().Uri).ToList();
        }

        public LikesDislikes GetUsersTrackFeelings(PartyGoer user)
        {
            return _usersLikesDislikes.GetUsersTrackFeelings(user);
        }

        public async Task UserLikesTrackAsync(PartyGoer user, string trackUri, string partyCode)
        {
            if (!_usersLikesDislikes.DoesUserLikeTrack(user, trackUri))
            {
                _usersLikesDislikes.UserLikesTrack(user, trackUri);

                _tracks.Find(p => p.GetTrackWithoutFeelings().Uri.Equals(trackUri, StringComparison.OrdinalIgnoreCase)).UserLikesTrack();

                _tracks.Sort(new ReorderQueueComparer());

                await DomainEvents.RaiseAsync(new UpdateQueue { Tracks = _tracks.Select(p => p.GetTrackWithoutFeelings()).ToList(), PartyCode = partyCode });
            }
        }

        public async Task UserDislikesTrackAsync(PartyGoer user, string trackUri, int listenerCount, string partyCode)
        {
            if (!_usersLikesDislikes.DoesUserDislikeTrack(user, trackUri))
            {
                _usersLikesDislikes.UserDislikesTrack(user, trackUri);

                if (_tracks.Find(p => p.GetTrackWithoutFeelings().Uri.Equals(trackUri, StringComparison.OrdinalIgnoreCase)).DislikeCount() + 1 > listenerCount * 0.5)
                {
                    _tracks.RemoveAll(p => p.GetTrackWithoutFeelings().Uri.Equals(trackUri, StringComparison.OrdinalIgnoreCase));

                    await DomainEvents.RaiseAsync(new UpdateQueue { Tracks = _tracks.Select(p => p.GetTrackWithoutFeelings()).ToList(), PartyCode = partyCode });
                }
            }
        }

        public List<string> GetLikedTrackUris()
        {
            List<string> likedTrackUris = new List<string>();

            foreach (TrackWithFeelings track in _tracks)
            {
                if (track.DoUsersLikeSong())
                {
                    likedTrackUris.Add(track.GetTrackWithoutFeelings().Uri);
                }
            }

            return likedTrackUris;
        }

        public bool HasExplicitTracks()
        {
            return _tracks.Any(p => p.GetTrackWithoutFeelings().Explicit);
        }

        public bool HasNextTrack()
        {
            return _tracks.Any();
        }

        public void QueueTrack(Track track)
        {
            _tracks.Add(new TrackWithFeelings(track));
        }
    }
}
