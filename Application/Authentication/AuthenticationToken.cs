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
        public string AccessToken { get; }
        public string RefreshToken { get; }
        public int ExpiresInXSeconds { get; }
        public DateTime TimeAccessTokenWasRequested { get; }

        public bool IsAccessTokenExpired()
        {
            if ((DateTime.Now - TimeAccessTokenWasRequested).TotalSeconds > ExpiresInXSeconds)
                return true;
            else
                return false;
        }
    }
}
