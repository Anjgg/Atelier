using Atelier.Api._Data;
using Atelier.Api._DTOs;
using Atelier.Api._Entities;
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

        public PlayerService(AppDbContext context)
        {
            _context = context;
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
            return MapToPlayerDto(player);
        }

        private PlayerDto MapToPlayerDto(Player p)
        {
            return new PlayerDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                ShortName = $"{p.FirstName.Substring(0, 1).ToUpper()}.{p.LastName.Substring(0, 3).ToUpper()}",
                Sex = p.Sex == Sex.Male ? "M" : "F",
                Country = new CountryDto
                {
                    Code = p.Country.Code,
                    Picture = p.Country.Picture
                },
                Picture = p.Picture,
                Data = new PlayerDataDto
                {
                    Rank = p.Data.Rank,
                    Points = p.Data.Points,
                    Weight = p.Data.Weight,
                    Height = p.Data.Height,
                    Age = p.Data.Age,
                    Last = p.Data.LastResults
                                .OrderBy(r => r.Order)
                                .Select(r => r.Result ? 1 : 0)
                                .ToList()
                }
            };
        }
    }
}
