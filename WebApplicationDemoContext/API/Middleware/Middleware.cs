using System.IdentityModel.Tokens.Jwt;

namespace WebApplicationDemoContext.API.Middleware
{
    public class Middleware : IMiddleware
    {
        private readonly ILogger<Middleware> _logger;

        public Middleware(ILogger<Middleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger?.LogWarning("Unauthorized request - missing or invalid token.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            var token = authHeader.Substring(7);

            var userId = GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId))
            {
                _logger?.LogWarning("Unauthorized request - Unable to extract userID.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
            
            context.Items["userID"] = userId;

            await next(context);
        }

        private string? GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    _logger?.LogWarning("Token parsing failed.");
                    return null;
                }

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger?.LogWarning("Token is missing 'sub' claim.");
                    return null;
                }

                return userId;
            }
            catch
            {
                _logger?.LogWarning("Token validation failed.");
                return null;
            }
        }
    }
}
