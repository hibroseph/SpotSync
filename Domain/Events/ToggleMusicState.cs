using SpotSync.Domain.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Events
{
    public class ToggleMusicState : IDomainEvent
    {
        public PartyGoer Listener { get; set; }
        public PlaybackState State { get; set; }
    }
}
