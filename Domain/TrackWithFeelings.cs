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

        public TrackWithFeelings(Track track)
        {
            _track = track;
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
    }
}
