
using SpotSync.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace SpotSync.Models.Party
{
    public class PartyModel
    {
        public string PartyCode { get; set; }
        public List<SongModel> SuggestedSongs { get; set; }
        public bool IsUserListening { get; set; }
    }
}
