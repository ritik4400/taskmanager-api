using Microsoft.AspNetCore.Mvc;
using taskmanager_api.Services.Interfaces;

namespace taskmanager_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IHealthService _healthService;

    public HealthController(IHealthService healthService)
    {
        _healthService = healthService;
    }


    [HttpGet]
    public IActionResult GetHealth()
    {
        var result = _healthService.GetHealthStatus();

        return Ok(result);
    }

    [HttpGet("db")]
    public async Task<IActionResult> GetDb()
    {
        var connected = await _healthService.CanConnectToDatabaseAsync();

        if (!connected)
            return StatusCode(503, new { database = "Unreachable" });

        return Ok(new { database = "Connected" });
    }
}