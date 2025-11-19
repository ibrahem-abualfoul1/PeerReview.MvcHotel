using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;

namespace PeerReview.MvcHotel.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SessionValidationMiddleware> _logger;

        public SessionValidationMiddleware(RequestDelegate next, ILogger<SessionValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null || endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            var jwt = context.Session.GetString("jwt");
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogInformation("Missing session for path {Path}; redirecting to login", context.Request.Path);

                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }

                var requested = context.Request.Path + context.Request.QueryString;
                var redirect = string.IsNullOrEmpty(requested)
                    ? "/Auth/Login"
                    : $"/Auth/Login?returnUrl={Uri.EscapeDataString(requested)}";

                context.Response.Redirect(redirect);
                return;
            }

            await _next(context);
        }
    }
}
