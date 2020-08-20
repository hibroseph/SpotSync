using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Web;

namespace SpotSync.Models.Account
{
    public class LoginWithSpotifyModel
    {
        public string SpotifyAuthenticationUrl { get; set; }
        public string SpaceDelimitedScopes { get; set; }
        public string ClientId { get; set; }
        public string RedirectUrl { get; set; }

        public string CreateSpotifyAuthenicateUrl()
        {
            return $"{SpotifyAuthenticationUrl}?response_type=code&client_id={ClientId}&scope={HttpUtility.UrlEncode(SpaceDelimitedScopes)}&redirect_uri={RedirectUrl}";
        }
    }
}
