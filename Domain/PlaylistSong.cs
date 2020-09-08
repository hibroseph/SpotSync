using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SpotSync.Domain
{
    public class PlaylistSong : Song
    {
        private Stopwatch _stopwatch;
        private long _timeLeft;
        public bool IsPaused() => _stopwatch.IsRunning;
    }
}
