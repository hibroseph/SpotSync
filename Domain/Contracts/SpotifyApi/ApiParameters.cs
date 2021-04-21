using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotifyApi
{
    public class ApiParameters
    {
        public Dictionary<string, string> Parameters { get; set; }
        public Dictionary<string, string> Keys { get; set; }
    }

}
