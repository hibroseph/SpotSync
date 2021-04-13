using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotSync.Domain
{
    public class UsersLikesDislikes
    {
        private Dictionary<string, List<string>> _usersLikedTracks;
        private Dictionary<string, List<string>> _usersDislikedTracks;

        public UsersLikesDislikes()
        {
            _usersLikedTracks = new Dictionary<string, List<string>>();
            _usersDislikedTracks = new Dictionary<string, List<string>>();
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

        public LikesDislikes GetUsersTrackFeelings(PartyGoer user)
        {
            List<string> likedTracks;
            List<string> dislikedTracks;

            if (HasUserDislikedTracksBefore(user))
            {
                dislikedTracks = _usersDislikedTracks[user.GetId()];
            }
            else
            {
                dislikedTracks = new List<string>();
            }

            if (HasUserLikedTracksBefore(user))
            {
                likedTracks = _usersLikedTracks[user.GetId()];

            }
            else
            {
                likedTracks = new List<string>();

            }

            return new LikesDislikes(likedTracks, dislikedTracks);
        }

        public void UserDislikesTrack(PartyGoer user, string trackUri)
        {
            StoreUsersDislikedTrack(user, trackUri);


        }

        private void StoreUsersDislikedTrack(PartyGoer user, string trackUri)
        {
            if (HasUserDislikedTracksBefore(user))
            {
                if (!_usersDislikedTracks[user.GetId()].Contains(trackUri))
                {
                    _usersDislikedTracks[user.GetId()].Add(trackUri);
                }
            }
            else
            {
                _usersDislikedTracks[user.GetId()] = new List<string> { trackUri };
            }

            // If the user used to dislike this song, remove it from that list
            if (DoesUserLikeTrack(user, trackUri))
            {
                UserDoesntLikeTrackAnymore(user, trackUri);
            }
        }

        public void UserLikesTrack(PartyGoer user, string trackUri)
        {
            StoreUsersLikedTrack(user, trackUri);

        }

        private void StoreUsersLikedTrack(PartyGoer user, string trackUri)
        {
            if (HasUserLikedTracksBefore(user))
            {
                if (!_usersLikedTracks[user.GetId()].Contains(trackUri))
                {
                    _usersLikedTracks[user.GetId()].Add(trackUri);
                }
            }
            else
            {
                _usersLikedTracks[user.GetId()] = new List<string> { trackUri };
            }

            // If the user used to dislike this song, remove it from that list
            if (DoesUserDislikeTrack(user, trackUri))
            {
                UserDoesntDislikeTrackAnymore(user, trackUri);
            }
        }

        private bool HasUserDislikedTracksBefore(PartyGoer user)
        {
            return _usersDislikedTracks.ContainsKey(user.GetId());
        }
        private bool HasUserLikedTracksBefore(PartyGoer user)
        {
            return _usersLikedTracks.ContainsKey(user.GetId());
        }

        private void UserDoesntDislikeTrackAnymore(PartyGoer user, string trackUri)
        {
            if (_usersDislikedTracks.ContainsKey(user.GetId()))
            {
                _usersDislikedTracks[user.GetId()].Remove(trackUri);
            }
        }

        public bool DoesUserDislikeTrack(PartyGoer partyGoer, string trackUri)
        {
            if (_usersDislikedTracks.ContainsKey(partyGoer.GetId()))
            {
                return _usersDislikedTracks[partyGoer.GetId()].Contains(trackUri);
            }

            return false;
        }

        private void UserDoesntLikeTrackAnymore(PartyGoer user, string trackUri)
        {
            if (_usersLikedTracks.ContainsKey(user.GetId()))
            {
                _usersLikedTracks[user.GetId()].Remove(trackUri);
            }
        }

        public bool DoesUserLikeTrack(PartyGoer partyGoer, string trackUri)
        {
            if (_usersLikedTracks.ContainsKey(partyGoer.GetId()))
            {
                return _usersLikedTracks[partyGoer.GetId()].Contains(trackUri);
            }

            return false;
        }

    }
}
