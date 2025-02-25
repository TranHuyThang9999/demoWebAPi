using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using WebApplicationDemoContext.Services.IServices;

namespace WebApplicationDemoContext.API.Middleware
{
    public class Middleware : IMiddleware
    {
        private readonly ILogger<Middleware> _logger;
        private readonly IServiceJWT _serviceJwt;
        private readonly IServiceUser _serviceUser;

        public Middleware(ILogger<Middleware> logger, IServiceJWT serviceJwt, IServiceUser serviceUser)
        {
            _logger = logger;
            _serviceJwt = serviceJwt;
            _serviceUser = serviceUser;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                var endpoint = context.GetEndpoint();
                if (endpoint != null && endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
                {
                    await next(context);
                    return;
                }


                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _logger?.LogWarning("Unauthorized request - missing or invalid token.");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                var token = authHeader.Substring(7);
                var tokenInfo = _serviceJwt.GetUserIdFromToken(token);


                if (!tokenInfo.TryGetValue("userID", out var userId) || string.IsNullOrEmpty(userId))
                {
                    _logger?.LogWarning("Unauthorized request - Unable to extract userID.");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                var user = _serviceUser.GetUserByUserId(int.Parse(userId));
                if (user?.Result.Data == null)
                {
                    _logger?.LogWarning("Unauthorized request - Unable to extract user.");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }


                if (tokenInfo.TryGetValue("lastPasswordUpdate", out var lastPasswordUpdate) &&
                    !string.IsNullOrEmpty(lastPasswordUpdate))
                {
                    if (DateTime.TryParse(lastPasswordUpdate, out var tokenDateTime))
                    {
                        var tokenTimestamp = new DateTimeOffset(tokenDateTime).ToUnixTimeSeconds();
                        var userTimestamp = new DateTimeOffset(user.Result.Data.Updated).ToUnixTimeSeconds();

                        if (tokenTimestamp != userTimestamp)
                        {
                            _logger?.LogWarning("Unauthorized request - Password has been changed.");
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync(
                                $"Unauthorized - Password changed. {tokenTimestamp} : {userTimestamp}");
                            return;
                        }
                    }
                }


                context.Items["userID"] = userId;
                await next(context);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An unexpected error occurred in authentication middleware.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync($"Internal Server Error : {ex.Message}");
            }
        }
    }
}