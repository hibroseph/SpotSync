using NUnit.Framework;
using SpotSync.Domain;
using SpotSync.Domain.Contracts.SpotifyApi.Models;
using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotSync.Domain.PartyAggregate;

namespace SpotSync.Tests.Unit_Tests
{
    [TestFixture]
    public class PartyTests
    {
        private const bool EXPLICIT = false;
        private const string MARKET = "US";
        private const string PRODUCT = "Premium";

        [Test]
        public void NoHost_NewPartyGoer_IsNewHost()
        {
            PartyGoer partyGoer = new PartyGoer("hello", EXPLICIT, MARKET, PRODUCT);
            PartyGoer partyGoer1 = new PartyGoer("hello2", EXPLICIT, MARKET, PRODUCT);

            Party party = new Party(partyGoer);

            party.LeaveParty(partyGoer);

            Assert.IsNull(party.GetHost());

            party.JoinParty(partyGoer1);

            Assert.IsTrue(party.IsHost(partyGoer1));
        }

        [Test]
        public async Task SkipSong_HostSkipsSong_SongIsSkipped()
        {
            PartyGoer host = new PartyGoer("Kip", EXPLICIT, MARKET, PRODUCT);

            Party party = new Party(host);

            List<Domain.Track> tracks = GetTracks();

            await party.AddNewQueueAsync(tracks);

            Assert.AreEqual(party.GetCurrentSong().Id, tracks.ElementAt(0).Id);

            await party.RequestSkipAsync(host);

            Assert.AreEqual(party.GetCurrentSong().Id, tracks.ElementAt(1).Id);
        }

        [Test]
        public async Task SkipSong_ListenerSkipsSong_SongIsNotSkipped()
        {
            PartyGoer host = new PartyGoer("Kip", EXPLICIT, MARKET, PRODUCT);
            PartyGoer listener = new PartyGoer("Matt", EXPLICIT, MARKET, PRODUCT);

            Party party = new Party(host);

            party.JoinParty(listener);

            List<Domain.Track> tracks = GetTracks();

            await party.AddNewQueueAsync(tracks);

            Assert.AreEqual(party.GetCurrentSong().Id, tracks.ElementAt(0).Id);

            await party.RequestSkipAsync(listener);

            Assert.AreEqual(party.GetCurrentSong().Id, tracks.ElementAt(0).Id);
        }

        [Test]
        public async Task SkipSong_EndOfPlaylist_GeneratesNewPlaylistEvent()
        {
            DomainEvents.Register<QueueEnded>((p) =>
            {
                Assert.Pass();
            });

            PartyGoer host = new PartyGoer("Kip", EXPLICIT, MARKET, PRODUCT);

            Party party = new Party(host);

            List<Domain.Track> tracks = GetTracks();

            await party.AddNewQueueAsync(tracks);

            Assert.AreEqual(party.GetCurrentSong().Id, tracks.ElementAt(0).Id);

            await party.RequestSkipAsync(host);
            await party.RequestSkipAsync(host);

            Assert.Fail();
        }

        [Test]
        public async Task DownVote_HostDownVotesSong_SongSkips()
        {
            PartyGoer host = new PartyGoer("Kip", EXPLICIT, MARKET, PRODUCT);

            Party party = new Party(host);

            List<Domain.Track> tracks = GetTracks();

            await party.AddNewQueueAsync(tracks);

            Assert.AreEqual(party.GetCurrentSong().Id, tracks.ElementAt(0).Id);

            await party.UserDislikesTrackAsync(host, tracks.ElementAt(0).Id);

            Assert.AreEqual(party.GetCurrentSong().Id, tracks.ElementAt(1).Id);

        }

        [Test]
        public async Task DownVote_ListenerDownVotesSong_SongSkips()
        {
            PartyGoer host = new PartyGoer("Kip", EXPLICIT, MARKET, PRODUCT);
            PartyGoer listener = new PartyGoer("Matt", EXPLICIT, MARKET, PRODUCT);

            Party party = new Party(host);

            party.JoinParty(listener);

            List<Domain.Track> tracks = GetTracks();

            await party.AddNewQueueAsync(tracks);

            Assert.AreEqual(party.GetCurrentSong().Id, tracks.ElementAt(0).Id);

            await party.UserDislikesTrackAsync(listener, tracks.ElementAt(0).Id);

            Assert.AreEqual(party.GetCurrentSong().Id, tracks.ElementAt(1).Id);
        }


        private List<Domain.Track> GetTracks()
        {
            return new List<Domain.Track>() {
                new Domain.Track {
                    AlbumImageUrl = "hello.com",
                    Artists = new List<Artist>{ new Artist{ Name = "Kipelicious", Id = "123" } },
                    Explicit = true,
                    Length = 500000,
                    Name = "I Love Kip",
                    Id = "xyz123"
            },
                new Domain.Track {
                    AlbumImageUrl = "goodbye.com",
                    Artists =new List<Artist>{ new Artist{ Name="Mattelicious", Id = "321"} },
                    Explicit = true,
                    Length = 500000,
                    Name = "Kip is the best",
                    Id = "zyx321"
            } };
        }
    }
}
