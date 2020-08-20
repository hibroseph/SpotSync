using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.DTO
{
    public class PartyDTO
    {
        public Guid Id { get; set; }
        public PartyGoer Host { get; set; }
        public List<PartyGoerDTO> Attendees { get; set; }
        public string PartyCode { get; set; }
    }
}
