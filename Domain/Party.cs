using SpotSync.Domain.Events;
using SpotSync.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Domain
{
    public class Party
    {
        public Playlist Playlist;
        public Guid Id { get; }
        public PartyGoer Host { get; set; }
        public List<PartyGoer> Listeners { get; }
        public string PartyCode { get; }
        private const int LENGTH_OF_PARTY_CODE = 6;
        public bool HasExplicitSongs() => Playlist.Queue.Any(p => p.Explicit == true);
        public Party(PartyGoer host)
        {
            Id = Guid.NewGuid();
            Host = host;
            Listeners = new List<PartyGoer>() { host };
            PartyCode = GeneratePartyCode();
            Playlist = new Playlist(Listeners, PartyCode);
        }

        public async Task TogglePlaybackAsync(PartyGoer partyGoer)
        {
            // Grabbing partier by reference, so any change I make to it will change it in the list
            PartyGoer partier = Listeners.Find(p => p.Equals(partyGoer));

            partier.PausedMusic = !partier.PausedMusic;

            await DomainEvents.RaiseAsync(new ToggleMusicState { Listener = partier, State = DetermineMusicState(partier.PausedMusic) });
        }

        private MusicState DetermineMusicState(bool isMusicPaused)
        {
            return isMusicPaused ? MusicState.Pause : MusicState.Play;
        }

        public async Task ModifyPlaylistAsync(RearrangeQueueRequest request)
        {
            await Playlist.ModifyQueueAsync(request);
        }

        public async Task ModifyPlaylistAsync(AddSongToQueueRequest request)
        {
            await Playlist.AddSongToQueueAsync(request);

            // Play the song that was added to queue automatically
            if (Playlist.CurrentSong == null)
            {
                await Playlist.StartAsync();
            }
        }

        public async Task DeletePlaylistAsync()
        {
            await Playlist?.DeleteAsync();
            Playlist = null;
        }

        public bool IsPartyPlayingMusic() => Playlist?.CurrentSong != null;

        public void JoinParty(PartyGoer partyGoer)
        {
            Listeners.Add(partyGoer);

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
