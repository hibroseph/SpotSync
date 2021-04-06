using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SpotSync.Domain
{
    class ReorderQueueComparer : IComparer<TrackWithFeelings>
    {
        public int Compare([AllowNull] TrackWithFeelings x, [AllowNull] TrackWithFeelings y)
        {
            // Sort by likes
            if ((x.LikeCount() - x.DislikeCount()) > (y.LikeCount() - y.DislikeCount()))
            {
                return -1;
            }
            else if ((x.LikeCount() - x.DislikeCount()) < (y.LikeCount() - y.DislikeCount()))
            {
                return 1;
            }

            // Sort by time added
            if (x.GetTimeLastUpvoted() > y.GetTimeLastUpvoted())
            {
                return 1;
            }
            else if (x.GetTimeLastUpvoted() < y.GetTimeLastUpvoted())
            {
                return -1;
            }

            return 0;
        }
    }
}
