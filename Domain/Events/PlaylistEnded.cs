﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Events
{
    public class PlaylistEnded : IDomainEvent
    {
        public string PartyCode { get; set; }
        public List<string> LikedTracksUris { get; set; }
    }
}
