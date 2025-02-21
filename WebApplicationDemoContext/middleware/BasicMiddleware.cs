
namespace WebApplicationDemoContext.Middleware
{
    public class BasicMiddleware : IMiddleware
    {
        private readonly ILogger<BasicMiddleware> _logger;

        public BasicMiddleware(ILogger<BasicMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string token = context.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
            {
                _logger?.LogWarning("Unauthorized request - missing or invalid token.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            _logger?.LogInformation($"BasicMiddleware Invoked - Token: {token}");

            context.Items["ContextValue"] = "This is a context value 22";

            await next(context);
        }
    }
}