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
        private DateTime _lastUpvoted;

        public TrackWithFeelings(Track track)
        {
            _lastUpvoted = DateTime.Now;
            _track = track;
        }

        public DateTime GetTimeLastUpvoted()
        {
            return _lastUpvoted;
        }

        public void UserLikesTrack()
        {
            _likes++;
            _lastUpvoted = DateTime.Now;
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
