using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Domain
{
    public class Party
    {
        private static Random Random = new Random();
        private const int LENGTH_OF_PARTY_CODE = 6;
        private Playlist _playlist;
        public Party(PartyGoer host)
        {
            Id = Guid.NewGuid();
            Host = host;
            Attendees = new List<PartyGoer>();
            PartyCode = GeneratePartyCode();
        }

        public void CreatePlaylist(Playlist playlist)
        {
            _playlist = playlist;
        }

        public void StartPlaylist()
        {
            _playlist.Start();
        }

        public Guid Id { get; }
        public PartyGoer Host { get; }
        public List<PartyGoer> Attendees { get; }
        public string PartyCode { get; }

        public void JoinParty(PartyGoer partyGoer)
        {
            Attendees.Add(partyGoer);
        }
        private static string GeneratePartyCode()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            // generate 6 random characters
            return new string(Enumerable.Repeat(chars, LENGTH_OF_PARTY_CODE).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
