using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SpotSync.Infrastructure.SpotifyApi
{
    public class SpotifyEndpoint
    {
        public string EndpointUrl { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public List<string> Keys { get; set; }
    }
}
