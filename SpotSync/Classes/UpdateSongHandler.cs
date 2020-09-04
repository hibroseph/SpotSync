using MediatR;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpotSync.Classes
{
    public class UpdateSongHandler : AsyncRequestHandler<UpdateSongRequest>
    {
        private ISpotifyHttpClient _spotifyHttpClient;
        public UpdateSongHandler(ISpotifyHttpClient spotifyHttpClient)
        {
            _spotifyHttpClient = spotifyHttpClient;
        }

        protected override async Task Handle(UpdateSongRequest request, CancellationToken cancellationToken)
        {
            List<Task<bool>> updateSongTasks = new List<Task<bool>>();
            foreach (var PartyGoer in request.Attendees)
            {
                updateSongTasks.Add(_spotifyHttpClient.UpdateSongForPartyGoerAsync(PartyGoer.Id, request.TrackUri, request.ProgressMs));
            }

            await Task.WhenAll(updateSongTasks);
        }
    }
}
