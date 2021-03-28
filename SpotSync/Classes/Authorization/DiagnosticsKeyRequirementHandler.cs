using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Classes.Authorization
{
    public class DiagnosticsKeyRequirementHandler : AuthorizationHandler<DiagnosticsKeyRequirement>
    {

        private const string API_KEY_HEADER_NAME = "X-DIAGNOSTIC-KEY";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DiagnosticsKeyRequirementHandler(IHttpContextAccessor accessor)
        {
            _httpContextAccessor = accessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DiagnosticsKeyRequirement requirement)
        {
            var apiKey = _httpContextAccessor.HttpContext.Request.Headers[API_KEY_HEADER_NAME].FirstOrDefault();
            if (apiKey != null && requirement.IsValidApiKey(apiKey))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
