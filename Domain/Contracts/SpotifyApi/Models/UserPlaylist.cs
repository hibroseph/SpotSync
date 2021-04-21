using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.SpotifyApi.Models
{
    public class UsersPlaylist
    {
        public string Id { get; set; }
        public List<Image> Images { get; set; }
        public string Name { get; set; }
    }
}
