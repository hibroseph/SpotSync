using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SpotSync.Infrastructure
{
    class SpotifyEndpoint
    {
        public string EndpointUrl { get; set; }
        public HttpMethod HttpMethod { get; set; }
    }
}
