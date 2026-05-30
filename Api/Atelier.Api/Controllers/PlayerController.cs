using Atelier.Api._DTOs;
using Atelier.Api._Swagger;
using Atelier.Api.Services;
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
        [SwaggerDocumentation(
            summary: "Fetch the list of all players",
            description: "Players are sorted from best rank to worst, separated by gender")]
        [ProducesResponseType(typeof(GetAllPlayersDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await _playerService.GetAllPlayersAsync();
            return Ok(players);
        }

        [HttpGet("{id}")]
        [SwaggerDocumentation(
            summary: "Fetch a player's information",
            description: "Returns the complete details of a player based on their ID")]
        [ProducesResponseType(typeof(PlayerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPlayerById(int id)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }

        [HttpPost]
        [SwaggerDocumentation(
            summary: "Add a new player",
            description: "Creates a new player with their statistics. Requires authentication."
        )]
        [ProducesResponseType(typeof(PlayerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var player = await _playerService.CreatePlayerAsync(dto);
                return CreatedAtAction(nameof(GetPlayerById), new { id = player.Id }, player);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
