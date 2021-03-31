using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace SpotSync.Domain
{
    public class Queue
    {
        private List<TrackWithFeelings> _tracks;
        private Dictionary<string, List<string>> _usersLikedTracks;
        private Dictionary<string, List<string>> _usersDislikedTracks;

        public Queue(List<Track> tracks)
        {
            _tracks = tracks.Select(p => new TrackWithFeelings(p)).ToList();
            _usersLikedTracks = new Dictionary<string, List<string>>();
            _usersDislikedTracks = new Dictionary<string, List<string>>();
        }

        public Queue()
        {
            _tracks = new List<TrackWithFeelings>();
            _usersLikedTracks = new Dictionary<string, List<string>>();
            _usersDislikedTracks = new Dictionary<string, List<string>>();
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
            foreach (KeyValuePair<string, List<string>> pairs in _usersLikedTracks)
            {
                if (_usersLikedTracks[pairs.Key].Count > 0)
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
            List<string> likedTracks;
            List<string> dislikedTracks;

            if (HasUserDislikedTracksBefore(user))
            {
                dislikedTracks = _usersDislikedTracks[user.Id];
            }
            else
            {
                dislikedTracks = new List<string>();
            }

            if (HasUserLikedTracksBefore(user))
            {
                likedTracks = _usersLikedTracks[user.Id];

            }
            else
            {
                likedTracks = new List<string>();

            }

            return new LikesDislikes(likedTracks, dislikedTracks);
        }

        public List<string> GetLikedTracksUris()
        {
            List<string> likedTracks = new List<string>();

            foreach (KeyValuePair<string, List<string>> pair in _usersLikedTracks)
            {
                likedTracks.AddRange(_usersLikedTracks[pair.Key]);
            }

            return likedTracks.Distinct().ToList();
        }

        public void UserDislikesTrack(PartyGoer user, string trackUri)
        {
            if (HasUserDislikedTracksBefore(user))
            {
                if (!_usersDislikedTracks[user.Id].Contains(trackUri))
                {
                    _usersDislikedTracks[user.Id].Add(trackUri);
                }
            }
            else
            {
                _usersDislikedTracks[user.Id] = new List<string> { trackUri };
            }

            // If the user used to dislike this song, remove it from that list
            if (DoesUserLikeTrack(user, trackUri))
            {
                UserDoesntLikeTrackAnymore(user, trackUri);
            }

            // reorder queue
        }

        public void UserLikesTrack(PartyGoer user, string trackUri)
        {
            if (HasUserLikedTracksBefore(user))
            {
                if (!_usersLikedTracks[user.Id].Contains(trackUri))
                {
                    _usersLikedTracks[user.Id].Add(trackUri);
                }
            }
            else
            {
                _usersLikedTracks[user.Id] = new List<string> { trackUri };
            }

            // If the user used to dislike this song, remove it from that list
            if (DoesUserDislikeTrack(user, trackUri))
            {
                UserDoesntDislikeTrackAnymore(user, trackUri);
            }

            // reorder queue
        }

        private bool HasUserDislikedTracksBefore(PartyGoer user)
        {
            return _usersDislikedTracks.ContainsKey(user.Id);
        }
        private bool HasUserLikedTracksBefore(PartyGoer user)
        {
            return _usersLikedTracks.ContainsKey(user.Id);
        }

        private void UserDoesntDislikeTrackAnymore(PartyGoer user, string trackUri)
        {
            if (_usersDislikedTracks.ContainsKey(user.Id))
            {
                _usersDislikedTracks[user.Id].Remove(trackUri);
            }
        }

        private bool DoesUserDislikeTrack(PartyGoer partyGoer, string trackUri)
        {
            if (_usersDislikedTracks.ContainsKey(partyGoer.Id))
            {
                return _usersDislikedTracks[partyGoer.Id].Contains(trackUri);
            }

            return false;
        }

        private void UserDoesntLikeTrackAnymore(PartyGoer user, string trackUri)
        {
            if (_usersLikedTracks.ContainsKey(user.Id))
            {
                _usersLikedTracks[user.Id].Remove(trackUri);
            }
        }

        private bool DoesUserLikeTrack(PartyGoer partyGoer, string trackUri)
        {
            if (_usersLikedTracks.ContainsKey(partyGoer.Id))
            {
                return _usersLikedTracks[partyGoer.Id].Contains(trackUri);
            }

            return false;
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
