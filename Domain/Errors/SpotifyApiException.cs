using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Errors
{
    public class SpotifyApiException : Exception
    {
        public SpotifyApiException(string message) : base(message)
        {

        }
    }
}
