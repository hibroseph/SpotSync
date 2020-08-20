using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts
{
    public interface IParty
    {
        public Guid Id { get; set; }
        public PartyGoer Host { get; set; }
        public List<PartyGoer> Attendees { get; set; }
        public string PartyCode { get; set; }
    }
}
