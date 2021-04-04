using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Events
{
    public class UpdateQueue : IDomainEvent
    {
        public List<Track> Tracks { get; set; }
        public string PartyCode { get; set; }
    }
}
