using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpotSync.Domain
{
    public class UsersLikesDislikes
    {
        private Dictionary<string, List<string>> _usersLikedTracks;
        private Dictionary<string, List<string>> _usersDislikedTracks;

        private Dictionary<string, int> _sumOfLikes;

        public UsersLikesDislikes()
        {
            _usersLikedTracks = new Dictionary<string, List<string>>();
            _usersDislikedTracks = new Dictionary<string, List<string>>();
            _sumOfLikes = new Dictionary<string, int>();
        }

        public Dictionary<string, int> GetTrackFeelings()
        {
            return _sumOfLikes;
        }

        public LikesDislikes GetUsersTrackFeelings(PartyGoer user)
        {
            return new LikesDislikes(_usersLikedTracks.GetUsersTracks(user), _usersDislikedTracks.GetUsersTracks(user));
        }

        public void UserDislikesTrack(PartyGoer user, string trackUri)
        {
            StoreUsersDislikedTrack(user, trackUri);
        }

        private void StoreUsersDislikedTrack(PartyGoer user, string trackUri)
        {
            _sumOfLikes.AdjustTrackFeelings(trackUri, Feelings.Dislike);
            _usersDislikedTracks.AddFeelings(user, trackUri);
            _usersLikedTracks.RemoveFeelings(user, trackUri);
        }

        public void UserLikesTrack(PartyGoer user, string trackUri)
        {
            StoreUsersLikedTrack(user, trackUri);
        }

        private void StoreUsersLikedTrack(PartyGoer user, string trackUri)
        {
            _sumOfLikes.AdjustTrackFeelings(trackUri, Feelings.Like);
            _usersLikedTracks.AddFeelings(user, trackUri);
            _usersDislikedTracks.RemoveFeelings(user, trackUri);
        }

        private void UpdateSongFeelingSum(string trackUri, Feelings feeling)
        {
            if (_sumOfLikes.ContainsKey(trackUri))
            {
                switch (feeling)
                {
                    case Feelings.Dislike:
                        _sumOfLikes[trackUri]--;
                        break;
                    case Feelings.Like:
                        _sumOfLikes[trackUri]++;
                        break;

                }
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

        public bool DoesUserLikeTrack(PartyGoer partyGoer, string trackUri)
        {
            if (_usersLikedTracks.ContainsKey(partyGoer.GetId()))
            {
                return _usersLikedTracks[partyGoer.GetId()].Contains(trackUri);
            }

            return false;
        }
    }

    public enum Feelings
    {
        Like = 0,
        Dislike = 1
    }

    public static class Extensions
    {
        public static void AdjustTrackFeelings(this Dictionary<string, int> dictionary, string trackUri, Feelings feeling)
        {
            if (dictionary.ContainsKey(trackUri))
            {
                switch (feeling)
                {
                    case Feelings.Like:
                        dictionary[trackUri]++;
                        break;
                    case Feelings.Dislike:
                        dictionary[trackUri]--;
                        break;
                    default:
                        throw new Exception($"Enum for Feelings does not exist for {feeling}");
                }
            }
            else
            {
                switch (feeling)
                {
                    case Feelings.Like:
                        dictionary.Add(trackUri, 1);
                        break;
                    case Feelings.Dislike:
                        dictionary.Add(trackUri, -1);
                        break;
                    default:
                        throw new Exception($"Enum for Feelings does not exist for {feeling}");
                }
            }
        }

        public static void AddFeelings(this Dictionary<string, List<string>> dictionary, PartyGoer partyGoer, string trackUri)
        {
            if (dictionary.ContainsKey(partyGoer.GetId()))
            {
                dictionary[partyGoer.GetId()].TryAdd(trackUri);
            }
            else
            {
                dictionary.Add(partyGoer.GetId(), new List<string> { trackUri });
            }
        }

        public static void RemoveFeelings(this Dictionary<string, List<string>> dictionary, PartyGoer partyGoer, string trackUri)
        {
            if (dictionary.ContainsKey(partyGoer.GetId()))
            {
                dictionary[partyGoer.GetId()].RemoveAll(p => p.Equals(trackUri));
            }
        }

        public static List<string> GetUsersTracks(this Dictionary<string, List<string>> dictionary, PartyGoer partyGoer)
        {
            if (dictionary.ContainsKey(partyGoer.GetId()))
            {
                return dictionary[partyGoer.GetId()];
            }
            else
            {
                return new List<string>();
            }
        }

        /*
         * This adds a string if it does not exist in the list 
         * */
        public static void TryAdd(this List<string> listy, string trackUri)
        {
            if (!listy.Contains(trackUri))
            {
                listy.Add(trackUri);
            }
        }
    }
}
