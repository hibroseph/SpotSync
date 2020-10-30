using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Events
{
    public class ConnectUsersSpotify : IDomainEvent
    {
        public PartyGoer User { get; set; }
    }
}
