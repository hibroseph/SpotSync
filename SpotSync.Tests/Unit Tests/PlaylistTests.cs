using NUnit.Framework;
using SpotSync.Domain;
using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpotSync.Tests.Unit_Tests
{
    class PlaylistTests
    {
        private const string PARTY_CODE = "123ABC";
        private List<Track> _songs;

        public PlaylistTests()
        {
            _songs = new List<Track>
            {
                new Track { Artist = "Artist1", Length = 1000, Name = "Title1", Uri = "TrackUri1"},
                new Track { Artist = "Artist2", Length = 5000, Name = "Title2", Uri = "TrackUri2"}
            };
        }

        [SetUp]
        public void ClearDomainEvents()
        {
            DomainEvents.ClearCallbacks();
        }

        [Test]
        [Ignore("Test needs to be rewritten")]
        public async Task PlaylistTwoSongs_NextSongValid()
        {

            Playlist playlist = new Playlist(_songs, new List<PartyGoer>(), PARTY_CODE);
            await playlist.StartAsync();

            await playlist.NextSongAsync();

            Assert.AreEqual(_songs.ElementAt(1), playlist.CurrentSong);
        }

        [Test]
        public void PlaylistTwoSongs_CurrentSongThrowsException()
        {
            Playlist playlist = new Playlist(_songs, new List<PartyGoer>(), PARTY_CODE);

            Assert.IsNull(playlist.CurrentSong);
        }

        [Test]
        public void PlaylistNoSongs_StartThrowsException()
        {
            Playlist playlist = new Playlist(new List<Track>(), new List<PartyGoer>(), PARTY_CODE);

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await playlist.StartAsync();
            });
        }

        [Test]
        [Ignore("Test needs to be rewritten")]
        public async Task PlaylistTwoSongs_NextSongStarts()
        {
            Playlist playlist = new Playlist(_songs, new List<PartyGoer>(), PARTY_CODE);

            await playlist.StartAsync();

            Thread.Sleep(2000);

            Assert.AreEqual(_songs.ElementAt(1), playlist.CurrentSong);
        }
    }
}
