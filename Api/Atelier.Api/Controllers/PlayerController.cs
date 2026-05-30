using Atelier.Api._DTOs;
using Atelier.Api._Swagger;
using Atelier.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atelier.Api.Controllers
{
    [ApiController]
    [Route("api/v1/players")]

    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerDocumentation(
            summary: "Fetch the list of all players",
            description: "Players are sorted from best rank to worst, separated by gender")]
        [ProducesResponseType(typeof(GetAllPlayersDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseType500Dto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await _playerService.GetAllPlayersAsync();
            
            return Ok(players);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerDocumentation(
            summary: "Fetch a player's information",
            description: "Returns the complete details of a player based on their ID")]
        [ProducesResponseType(typeof(PlayerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseType400Dto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseType404Dto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseType500Dto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPlayerById(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("id", "The id can't be negative");
                return ValidationProblem(ModelState);
            }

            var player = await _playerService.GetPlayerByIdAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerDocumentation(
            summary: "Add a new player",
            description: "Creates a new player with their statistics. Requires authentication.")]
        [ProducesResponseType(typeof(PlayerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseType400Dto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseType401Dto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseType403Dto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseType500Dto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerDto dto)
        {
            if (dto.Data.Last.Any(r => r != 0 && r != 1))
            {
                ModelState.AddModelError("data.last", "Last results must be 0 or 1");
                return ValidationProblem(ModelState);
            }

            var player = await _playerService.CreatePlayerAsync(dto);
            return CreatedAtAction(nameof(GetPlayerById), new { id = player.Id }, player);
        }
    }
}
