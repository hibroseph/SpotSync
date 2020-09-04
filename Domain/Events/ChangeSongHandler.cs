using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Events
{
    public class ChangeSongHandler : IHandles<ChangeSong>
    {
        public void Handle(ChangeSong args)
        {
            // updates the song
        }
    }
}
