using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain
{
    public class RearrangeQueueRequest
    {
        public int OldTrackIndex { get; set; }
        public int NewTrackIndex { get; set; }
        public string PartyCode { get; set; }
    }
}
