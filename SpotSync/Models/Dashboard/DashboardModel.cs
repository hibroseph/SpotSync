using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Models.Dashboard
{
    public class DashboardModel
    {
        public string Name { get; set; }
        public Song CurrentSong { get; set; }

        public string PrintCurrentSong()
        {
            return $"You are listening to {CurrentSong.Title} by {CurrentSong.Artist}";
        }

    }
}
