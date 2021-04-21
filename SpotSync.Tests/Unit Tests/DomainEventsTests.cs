using NUnit.Framework;
using NUnit.Framework.Internal;
using SpotSync.Domain;
using SpotSync.Domain.Contracts.SpotifyApi.Models;
using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Tests.Unit_Tests
{
    class DomainEventsTests
    {
        private const string PARTY_CODE = "abc123";
        private const int PROGRESS_MS = 3000;
        private const string SONG_ARTIST = "Kip";
        private const int SONG_LENGTH = 60000;
        private const string SONG_TITLE = "The Best Song";
        private const string SONG_URI = "song:uri:abc321";
        private const string PARTY_GOER_ID_1 = "ValidPartyId";

        [Test]
        public async Task ClearDomainEventsCallback()
        {
            DomainEvents.Register<ChangeTrack>(song => Assert.Fail());

            DomainEvents.ClearCallbacks();

            await DomainEvents.RaiseAsync(new ChangeTrack());

            Assert.Pass();
        }

        [Test]
        public async Task DomainEventsSuccessfullyRaisesDomainEvent()
        {
            Domain.Track song = new Domain.Track
            {
                Artists = new List<Artist> { new Artist { Id = "123", Name = SONG_ARTIST } },
                Length = SONG_LENGTH,
                Name = SONG_TITLE,
                Id = SONG_URI
            };

            DomainEvents.Register<ChangeTrack>(changedSong =>
            {
                Assert.AreEqual(PARTY_CODE, changedSong.PartyCode);
                Assert.AreEqual(song, changedSong.Track);
                Assert.AreEqual(PROGRESS_MS, changedSong.ProgressMs);
                Assert.Pass();
            });

            await DomainEvents.RaiseAsync(new ChangeTrack
            {
                PartyCode = PARTY_CODE,
                ProgressMs = PROGRESS_MS,
                Track = song,
                Listeners = new List<PartyGoer>
                {
                    new PartyGoer(PARTY_GOER_ID_1, false, "US", "premium")
                }
            });

            DomainEvents.ClearCallbacks();

            // If event doesn't get raised, 
            Assert.Fail();
        }

        [TearDown]
        public void CleanUp()
        {
            DomainEvents.ClearCallbacks();
        }
    }
}
