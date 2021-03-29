using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Models.Party
{
    public class LikedDislikedSongs
    {
        public List<string> LikedSongs { get; set; }
        public List<string> DislikedSongs { get; set; }
    }
}
