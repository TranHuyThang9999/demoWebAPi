using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationDemoContext.Services.IServices;

namespace WebApplicationDemoContext.API.Controller;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IServiceUser _serviceUser;
    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register()
    {
        //logic
        return Ok(new { message = "Register successful." });
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult GetDemoContext()
    {
        var contextValue = HttpContext.Items["ContextValue"]?.ToString();
        
        return Ok(new { Message = "Context value retrieved successfully", ContextValue = contextValue });
    }
}