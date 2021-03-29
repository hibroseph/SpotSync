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

        public void LikeSong(string trackUri)
        {
            _likedSongs.Add(trackUri);
        }

        public void DislikeSong(string trackUri)
        {
            _dislikedSongs.Add(trackUri);
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
