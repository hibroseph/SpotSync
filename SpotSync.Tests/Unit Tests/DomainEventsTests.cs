using NUnit.Framework;
using NUnit.Framework.Internal;
using SpotSync.Domain;
using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public void ClearDomainEventsCallback()
        {
            DomainEvents.Register<ChangeSong>(song => Assert.Fail());

            DomainEvents.ClearCallbacks();

            DomainEvents.Raise(new ChangeSong());

            Assert.Pass();
        }

        [Test]
        public void DomainEventsSuccessfullyRaisesDomainEvent()
        {
            Song song = new Song
            {
                Artist = SONG_ARTIST,
                Length = SONG_LENGTH,
                Title = SONG_TITLE,
                TrackUri = SONG_URI
            };

            DomainEvents.Register<ChangeSong>(changedSong =>
            {
                Assert.AreEqual(PARTY_CODE, changedSong.PartyCode);
                Assert.AreEqual(song, changedSong.Song);
                Assert.AreEqual(PROGRESS_MS, changedSong.ProgressMs);
                Assert.Pass();
            });

            DomainEvents.Raise(new ChangeSong
            {
                PartyCode = PARTY_CODE,
                ProgressMs = PROGRESS_MS,
                Song = song,
                Listeners = new List<PartyGoer>
                {
                    new PartyGoer(PARTY_GOER_ID_1)
                }
            });

            // If event doesn't get raised, 
            Assert.Fail();
        }
    }
}
