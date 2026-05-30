using Atelier.Api._DTOs;
using Atelier.Api._Swagger;
using Atelier.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atelier.Api.Controllers
{
    [ApiController]
    [Route("api/v1/stats")]
    public class StatsController: ControllerBase
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet]
        [SwaggerDocumentation(
            summary: "Fetch global statistics",
            description: "Returns the country with the highest win ratio, the average BMI and the median height of players"
        )]
        [ProducesResponseType(typeof(StatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _statsService.GetStatsAsync();
            if (stats == null)
            {
                return NotFound();
            }
            return Ok(stats);
        }
    }
}
