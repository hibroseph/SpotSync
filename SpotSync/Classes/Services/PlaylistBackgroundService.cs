using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpotSync.Classes.Services
{
    public class PlaylistBackgroundService : IHostedService, IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("Starting playlist background service");

            // Loop over all the playlists that have been created and see if we need to update the song

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("Ending playlist background service");

            return Task.CompletedTask;
        }
    }
}
