using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Events
{
    public class CreatePlaylist : IDomainEvent
    {
        public string PartyCode { get; set; }
        public List<Track> Songs { get; set; }
    }
}
