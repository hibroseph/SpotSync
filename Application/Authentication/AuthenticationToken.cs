using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Application.Authentication
{
    class AuthenticationToken
    {
        public AuthenticationToken(string accessToken, string refreshToken, int expiresInXSeconds)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresInXSeconds = expiresInXSeconds;
            TimeAccessTokenWasRequested = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }
        public Guid Id { get; }
        public string AccessToken { get; private set; }
        public string RefreshToken { get; }
        public int ExpiresInXSeconds { get; private set; }
        public DateTime TimeAccessTokenWasRequested { get; private set; }

        public bool IsAccessTokenExpired()
        {
            if ((DateTime.UtcNow - TimeAccessTokenWasRequested).TotalSeconds > ExpiresInXSeconds)
                return true;
            else
                return false;
        }

        public void UpdateAccessToken(string accessToken, int expiresInXSeconds)
        {
            AccessToken = accessToken;
            ExpiresInXSeconds = expiresInXSeconds;
            TimeAccessTokenWasRequested = DateTime.UtcNow;
        }
    }
}
