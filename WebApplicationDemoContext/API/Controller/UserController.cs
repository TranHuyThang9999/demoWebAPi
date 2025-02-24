using Microsoft.AspNetCore.Mvc;

namespace WebApplicationDemoContext.Controller;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet]
    public IActionResult GetDemoContext()
    {
        var contextValue = HttpContext.Items["ContextValue"]?.ToString();
        
        return Ok(new { Message = "Context value retrieved successfully", ContextValue = contextValue });
    }
}