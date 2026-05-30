using Atelier.Api._Data;
using Atelier.Api._DTOs;
using Atelier.Api._Entities;
using Atelier.Api.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Atelier.Api.Services
{
    public interface IPlayerService
    {
        Task<GetAllPlayersDto> GetAllPlayersAsync();
        Task<PlayerDto?> GetPlayerByIdAsync(int id);
    }

    public class PlayerService : IPlayerService
    {
        private readonly AppDbContext _context;
        private readonly IPlayerHelper _helper;

        public PlayerService(AppDbContext context, IPlayerHelper helper)
        {
            _context = context;
            _helper = helper;
        }

        public async Task<GetAllPlayersDto> GetAllPlayersAsync()
        {
            var players = await _context.Players
                .Include(p => p.Data)
                .OrderBy(p => p.Data.Rank)
                .ToListAsync();

            return new GetAllPlayersDto
            {
                Male = players
                    .Where(p => p.Sex == Sex.Male)
                    .Select(p => new PlayerNameDto
                    {
                        Id = p.Id,
                        Name = $"{p.FirstName} {p.LastName}"
                    })
                    .ToList(),
                Female = players
                    .Where(p => p.Sex == Sex.Female)
                    .Select(p => new PlayerNameDto
                    {
                        Id = p.Id,
                        Name = $"{p.FirstName} {p.LastName}"
                    })
                    .ToList()
            };
        }

        public async Task<PlayerDto?> GetPlayerByIdAsync(int id)
        {
            var player = await _context.Players
                .Include(p => p.Country)
                .Include(p => p.Data)
                .ThenInclude(p => p.LastResults)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (player == null) return null;
            return _helper.MapToPlayerDto(player);
        }
    }
}
