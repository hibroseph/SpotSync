using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Events
{
    public class ChangeSong : IDomainEvent
    {
        public string PartyCode { get; set; }
        public List<PartyGoer> Listeners { get; set; }
        public Song Song { get; set; }
        public int ProgressMs { get; set; }
    }
}
