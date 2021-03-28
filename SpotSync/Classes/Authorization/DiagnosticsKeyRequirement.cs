using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Classes.Authorization
{
    public class DiagnosticsKeyRequirement : IAuthorizationRequirement
    {
        private readonly string _key;

        public DiagnosticsKeyRequirement(string key)
        {
            _key = key;
        }

        public bool IsValidApiKey(string key)
        {
            return _key.Equals(key, StringComparison.OrdinalIgnoreCase);
        }
    }
}
