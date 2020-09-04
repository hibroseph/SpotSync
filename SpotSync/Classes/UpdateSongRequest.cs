using MediatR;
using SpotSync.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Classes
{
    public class UpdateSongRequest : IRequest
    {
        public string TrackUri { get; set; }
        public List<PartyGoer> Attendees { get; set; }
        public int ProgressMs { get; set; }
    }
}
