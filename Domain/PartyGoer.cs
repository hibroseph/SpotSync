using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Domain
{
    public class PartyGoer : IEquatable<PartyGoer>
    {
        private string _spotifyId;
        private bool _filterExplicitSongs;
        private bool _isMusicPaused;
        private string _market;
        private bool _hasPremium;
        private string _perferredDeviceId;

        public PartyGoer(string spotifyId, bool filterExplicitSongs, string market, string spotifyProduct)
        {
            _spotifyId = spotifyId;
            _filterExplicitSongs = filterExplicitSongs;
            _market = market;
            _hasPremium = spotifyProduct.Equals("premium", StringComparison.OrdinalIgnoreCase);
            _isMusicPaused = false;
        }

        public void AddPerferredDeviceId(string deviceId)
        {
            _perferredDeviceId = deviceId;
        }

        public void ToggleMusicPlaybackState()
        {
            _isMusicPaused = !_isMusicPaused;
        }

        public string GetPerferredDeviceId()
        {
            return _perferredDeviceId;
        }

        public string GetSpotifyId()
        {
            return _spotifyId;
        }

        public bool IsMusicPaused()
        {
            return _isMusicPaused;
        }

        public void PauseMusic()
        {
            _isMusicPaused = true;
        }

        public void StartMusic()
        {
            _isMusicPaused = false;
        }

        public string GetId()
        {
            return _spotifyId;
        }

        public string GetUsername()
        {
            return _spotifyId;
        }
        public bool CanListenToExplicitSongs()
        {
            return !_filterExplicitSongs;
        }

        public bool IsInMarket(string market)
        {
            return _market.Equals(market, StringComparison.OrdinalIgnoreCase);
        }

        public string GetMarket()
        {
            return _market;
        }

        public bool HasPremium()
        {
            return _hasPremium;
        }

        public bool Equals([AllowNull] PartyGoer other)
        {
            if (other == null) return false;

            return other.GetId().Equals(_spotifyId, StringComparison.OrdinalIgnoreCase);
        }
    }
}
