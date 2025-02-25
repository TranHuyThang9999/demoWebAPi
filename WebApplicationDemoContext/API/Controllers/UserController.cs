using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationDemoContext.Common;
using WebApplicationDemoContext.DTO;
using WebApplicationDemoContext.Services.IServices;

namespace WebApplicationDemoContext.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IServiceUser _serviceUser;

    public UserController(ILogger<UserController> logger, IServiceUser serviceUser)
    {
        _logger = logger;
        _serviceUser = serviceUser;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RequestUserCreate request)
    {
        try
        {
            var result = await _serviceUser.AddUser(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RequestUserLogin request)
    {
        try
        {
            var result = await _serviceUser.Login(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        catch (Exception e)
        {
            _logger.LogError("errror : ",e.Message);
            throw;
        }
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        try
        {
            if (!int.TryParse(HttpContext.Items["userID"]?.ToString(), out int userID))
            {
                return BadRequest(Result<string>.Fail("Invalid User ID format"));
            }
            var user = await _serviceUser.GetUserByUserID(userID); 
            
            return Ok(Result<object>.Ok(user, "User profile retrieved successfully"));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(500, Result<string>.Fail("Internal server error"));
            throw;
        }
    }
    [Authorize]
    [HttpGet]
    public IActionResult GetDemoContext()
    {
        var contextValue = HttpContext.Items["ContextValue"]?.ToString();
        
        return Ok(new { Message = "Context value retrieved successfully", ContextValue = contextValue });
    }
}