using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Models.Dashboard
{
    public class DashboardModel
    {
        public string Name { get; set; }
        public List<PreviewPartyModel> AvailableParties { get; set; }
        public List<PreviewPlaylistSong> SuggestedSongs { get; set; }
        public string RandomGreeting { get; set; }
    }
}
