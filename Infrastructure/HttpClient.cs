using SpotSync.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Infrastructure
{
    public class HttpClient : IHttpClient
    {
        private System.Net.Http.HttpClient _httpClient;

        public HttpClient()
        {
            _httpClient = new System.Net.Http.HttpClient();
        }
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            return await _httpClient.SendAsync(requestMessage);
        }
    }
}
