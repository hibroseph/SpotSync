using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.Contracts.SpotifyApi;
using SpotSync.Domain.Contracts.SpotifyApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using SpotibroModels = SpotSync.Domain.Contracts.SpotibroModels;
using SpotifyModels = SpotSync.Domain.Contracts.SpotifyApi.Models;

namespace SpotSync.Infrastructure.SpotifyApi
{
    class PersonalizationEndpoint
    {
        private ISpotifyHttpClient _spotifyHttpClient;
        private Dictionary<ApiEndpointType, SpotifyEndpoint> _apiEndpoints;
        private SpotifyToSpotibroModelMapper _mapper;
        private ILogService _logService;

        public PersonalizationEndpoint(ISpotifyHttpClient spotifyHttpClient, ILogService logService, Dictionary<ApiEndpointType, SpotifyEndpoint> apiEndpoints)
        {
            _spotifyHttpClient = spotifyHttpClient;
            _logService = logService;
            _mapper = new SpotifyToSpotibroModelMapper();
            _apiEndpoints = apiEndpoints;
        }

        public async Task<List<SpotibroModels.Artist>> GetUsersTopArtistsAsync(PartyGoer partier, int amount)
        {
            var response = await _spotifyHttpClient.SendHttpRequestAsync(partier, _apiEndpoints[ApiEndpointType.UsersTopArtists]);

            response.EnsureSuccessStatusCode();

            return _mapper.Convert((await response.Content.ReadFromJsonAsync<PagedObject<SpotifyModels.Artist>>()).Items);
        }
    }
}
