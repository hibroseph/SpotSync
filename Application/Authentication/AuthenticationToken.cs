using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Application.Authentication
{
    class AuthenticationToken
    {
        public AuthenticationToken(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
        public string AccessToken { get; }
        public string RefreshToken { get; }
    }
}
