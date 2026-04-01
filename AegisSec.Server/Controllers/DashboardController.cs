using AegisSec.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AegisSec.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly MongoService _mongoService;

        public DashboardController(MongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _mongoService.GetStatsAsync();
            return Ok(stats);
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs([FromQuery] int limit = 50)
        {
            var logs = await _mongoService.GetLogsAsync(limit);
            return Ok(logs);
        }

        [HttpGet("alerts")]
        public async Task<IActionResult> GetAlerts()
        {
            var alerts = await _mongoService.GetAlertsAsync();
            return Ok(alerts);
        }
    }
}
