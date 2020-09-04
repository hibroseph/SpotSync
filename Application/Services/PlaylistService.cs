using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Application.Services
{
    public class PlaylistService : IPlaylistService
    {
        private List<Playlist> _playlists;
        private ISpotifyHttpClient _spotifyHttpClient;
        private Random _random;

        public PlaylistService(ISpotifyHttpClient spotifyHttpClient)
        {
            _playlists = new List<Playlist>();
            _spotifyHttpClient = spotifyHttpClient;
            _random = new Random();
        }

        public void CreatePlaylist(string spotifyId, Party party, Playlist playlist)
        {

        }



        private List<string> GetNNumberOfTrackUris(List<string> topTrackUris, int selectNTracks)
        {
            return topTrackUris.OrderBy(p => _random.Next()).Take(selectNTracks).ToList();
        }

        public void CreatePlaylist(PartyCodeDTO partyCode)
        {
            throw new NotImplementedException();
        }

        public void StartPlaylist(Party party)
        {
            throw new NotImplementedException();
        }

        public void StartPlaylist(PartyCodeDTO partyCode)
        {
            throw new NotImplementedException();
        }

        public void StopPlaylist(Party party)
        {
            throw new NotImplementedException();
        }

        public void StopPlaylist(PartyCodeDTO partyCode)
        {
            throw new NotImplementedException();
        }
    }
}
