using SpotSync.Domain.Contracts;
using SpotSync.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Persistence
{
    public class PartyGoerRepository : IPartyGoerRepository
    {
        private List<PartyGoer> PartyGoers = new List<PartyGoer>();

        public void Add(PartyGoer item)
        {
            PartyGoers.Add(item);
        }

        public bool Exists(PartyGoer item)
        {
            var partyGoers = PartyGoers.FindAll(p => p.Id == item.Id);

            return partyGoers.Count == 1 ? true : throw new Exception($"There was {partyGoers.Count} party goers in the database with the same ID: {item.Id}");

        }

        public PartyGoer Get(PartyGoer item)
        {
            return PartyGoers.Find(p => p.Id == item.Id);
        }

        public void Remove(PartyGoer item)
        {
            PartyGoers.RemoveAll(p => p.Id == item.Id);
        }
    }
}
