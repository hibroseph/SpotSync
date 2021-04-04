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
            if (x.LikeCount() > y.LikeCount())
            {
                return -1;
            }
            else if (x.LikeCount() < y.LikeCount())
            {
                return 1;
            }

            return 0;
        }
    }
}
