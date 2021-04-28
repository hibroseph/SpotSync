using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Infrastructure.SpotifyApi
{
    public enum ApiEndpointType
    {
        CurrentSong = 1,
        UserInformation = 2,
        PlaySong = 3,
        Token = 4,
        GetTopTracks = 5,
        GetRecommendedTracks = 6,
        GetUserDevices = 7,
        SearchSpotify = 8,
        PausePlayback = 9,
        GetUserPlaylists = 10,
        GetPlaylistItems = 11,
        ArtistInformation = 12,
        ArtistTopTracks = 13,
        UsersTopArtists = 14
    }
}
