using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery] bool full = false)
    {
        if (full)
        {
            return Ok(new
            {
                status = "ok",
                time = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                machine = Environment.MachineName
            });
        }
        return Ok(new { status = "ok", time = DateTime.UtcNow });
    }
    
    [HttpGet("ping")]
    public string GetPong()
    {
        return "pong";
    }
}