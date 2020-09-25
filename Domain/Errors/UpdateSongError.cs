using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Errors
{
    public class UpdateSongError
    {
        public UpdateSongError(string message)
        {
            FriendlyMessage = message;
        }

        public string FriendlyMessage { get; set; }
    }
}
