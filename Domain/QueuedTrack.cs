using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain
{
    public class QueuedTrack : Track
    {
        public string AddedBy { get; set; }
    }
}
