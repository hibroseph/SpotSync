using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Domain
{
    public class PartyGoer
    {
        public PartyGoer(string id) => Id = id;
        public string Id { get; }
    }
}
