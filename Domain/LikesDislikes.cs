using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SpotSync.Domain
{
    public class LikesDislikes
    {
        private readonly List<string> _likedSongs;
        private readonly List<string> _dislikedSongs;

        public LikesDislikes()
        {
            _likedSongs = new List<string>();
            _dislikedSongs = new List<string>();
        }

        public LikesDislikes(List<string> likedSongsUris, List<string> dislikedSongUris)
        {
            _likedSongs = likedSongsUris;
            _dislikedSongs = dislikedSongUris;
        }

        public void LikeSong(string trackUri)
        {
            if (_dislikedSongs.Contains(trackUri))
            {
                _dislikedSongs.Remove(trackUri);
            }

            if (!_likedSongs.Contains(trackUri))
            {
                _likedSongs.Add(trackUri);
            }
        }

        public void DislikeSong(string trackUri)
        {
            if (_likedSongs.Contains(trackUri))
            {
                _likedSongs.Remove(trackUri);
            }

            if (!_dislikedSongs.Contains(trackUri))
            {
                _dislikedSongs.Add(trackUri);
            }
        }


        public List<string> GetLikedSongs()
        {
            return _likedSongs;
        }

        public List<string> GetDislikedSongs()
        {
            return _dislikedSongs;
        }
    }
}
