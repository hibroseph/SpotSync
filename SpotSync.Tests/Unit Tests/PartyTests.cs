using NUnit.Framework;
using SpotSync.Domain;
using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Tests.Unit_Tests
{
    [TestFixture]
    public class PartyTests
    {
        [Test]
        public void NoHost_NewPartyGoer_IsNewHost()
        {
            PartyGoer partyGoer = new PartyGoer("hello");
            PartyGoer partyGoer1 = new PartyGoer("hello2");

            Party party = new Party(partyGoer);

            party.LeaveParty(partyGoer);

            Assert.IsNull(party.GetHost());

            party.JoinParty(partyGoer1);

            Assert.IsTrue(party.IsHost(partyGoer1));
        }

        [Test]
        public async Task SkipSong_HostSkipsSong_SongIsSkipped()
        {
            PartyGoer host = new PartyGoer("Kip");

            Party party = new Party(host);

            List<Track> tracks = GetTracks();

            await party.AddNewQueueAsync(tracks);

            Assert.AreEqual(party.GetCurrentSong().Uri, tracks.ElementAt(0).Uri);

            await party.RequestSkipAsync(host);

            Assert.AreEqual(party.GetCurrentSong().Uri, tracks.ElementAt(1).Uri);
        }

        [Test]
        public async Task SkipSong_ListenerSkipsSong_SongIsNotSkipped()
        {
            PartyGoer host = new PartyGoer("Kip");
            PartyGoer listener = new PartyGoer("Matt");

            Party party = new Party(host);

            party.JoinParty(listener);

            List<Track> tracks = GetTracks();

            await party.AddNewQueueAsync(tracks);

            Assert.AreEqual(party.GetCurrentSong().Uri, tracks.ElementAt(0).Uri);

            await party.RequestSkipAsync(listener);

            Assert.AreEqual(party.GetCurrentSong().Uri, tracks.ElementAt(0).Uri);
        }

        [Test]
        public async Task SkipSong_EndOfPlaylist_GeneratesNewPlaylistEvent()
        {
            DomainEvents.Register<QueueEnded>((p) =>
            {
                Assert.Pass();
            });

            PartyGoer host = new PartyGoer("Kip");

            Party party = new Party(host);

            List<Track> tracks = GetTracks();

            await party.AddNewQueueAsync(tracks);

            Assert.AreEqual(party.GetCurrentSong().Uri, tracks.ElementAt(0).Uri);

            await party.RequestSkipAsync(host);
            await party.RequestSkipAsync(host);

            Assert.Fail();
        }

        [Test]
        public async Task DownVote_HostDownVotesSong_SongSkips()
        {
            PartyGoer host = new PartyGoer("Kip");

            Party party = new Party(host);

            List<Track> tracks = GetTracks();

            await party.AddNewQueueAsync(tracks);

            Assert.AreEqual(party.GetCurrentSong().Uri, tracks.ElementAt(0).Uri);

            await party.UserDislikesTrackAsync(host, tracks.ElementAt(0).Uri);

            Assert.AreEqual(party.GetCurrentSong().Uri, tracks.ElementAt(1).Uri);

        }

        [Test]
        public async Task DownVote_ListenerDownVotesSong_SongSkips()
        {
            PartyGoer host = new PartyGoer("Kip");
            PartyGoer listener = new PartyGoer("Matt");

            Party party = new Party(host);

            party.JoinParty(listener);

            List<Track> tracks = GetTracks();

            await party.AddNewQueueAsync(tracks);

            Assert.AreEqual(party.GetCurrentSong().Uri, tracks.ElementAt(0).Uri);

            await party.UserDislikesTrackAsync(listener, tracks.ElementAt(0).Uri);

            Assert.AreEqual(party.GetCurrentSong().Uri, tracks.ElementAt(1).Uri);
        }


        private List<Track> GetTracks()
        {
            return new List<Track>() {
                new Track {
                    AlbumImageUrl = "hello.com",
                    Artist = "Kipelicious",
                    Explicit = true,
                    Length = 500000,
                    Name = "I Love Kip",
                    Uri = "xyz123"
            },
                new Track {
                    AlbumImageUrl = "goodbye.com",
                    Artist = "Mattelicious",
                    Explicit = true,
                    Length = 500000,
                    Name = "Kip is the best",
                    Uri = "zyx321"
            } };
        }
    }
}
