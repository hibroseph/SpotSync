using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Domain
{
    public class PartyGoer : IEquatable<PartyGoer>
    {
        public PartyGoer(string id) => Id = id;
        public string Id { get; }

        public bool Equals([AllowNull] PartyGoer other)
        {
            return other.Id.Equals(Id, StringComparison.OrdinalIgnoreCase);
        }
    }
}
