using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace SpotSync.Domain
{
    public class TrackWithFeelings
    {
        private Track _track;
        private int _likes;
        private int _dislikes;
        private DateTime _timeAdded;

        public TrackWithFeelings(Track track)
        {
            _timeAdded = DateTime.Now;
            _track = track;
        }

        public DateTime GetTimeAdded()
        {
            return _timeAdded;
        }

        public void UserLikesTrack()
        {
            _likes++;
        }

        public void UserDislikesTrack()
        {
            _dislikes++;
        }

        public Track GetTrackWithoutFeelings()
        {
            return _track;
        }

        public bool DoUsersLikeSong()
        {
            return _likes > 0;
        }

        public int DislikeCount()
        {
            return _dislikes;
        }

        public int LikeCount()
        {
            return _likes;
        }
    }
}
