using Atelier.Api._Data;
using Atelier.Api._DTOs;
using Atelier.Api._Entities;
using Atelier.Api._Exception;
using Atelier.Api.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Atelier.Api.Services
{
    public interface IPlayerService
    {
        Task<GetAllPlayersDto> GetAllPlayersAsync();
        Task<PlayerDto> GetPlayerByIdAsync(int id);
        Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto);
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

            if (players.Count == 0 || players == null)
            {
                throw new NoDataException("No data has been retrieved");
            }

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

        public async Task<PlayerDto> GetPlayerByIdAsync(int id)
        {
            var player = await _context.Players
                .Include(p => p.Country)
                .Include(p => p.Data)
                .ThenInclude(p => p.LastResults)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new KeyNotFoundException();

            return _helper.MapToPlayerDto(player);
        }

        public async Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto)
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.Code == dto.CountryCode);

            if (country == null)
                country = new Country { Code = dto.CountryCode, Picture = string.Empty };

            var player = new Player
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                ShortName = $"{dto.FirstName.Substring(0, 1).ToUpper()}.{dto.LastName.Substring(0, 3).ToUpper()}",
                Sex = dto.Sex == "M" ? Sex.Male : Sex.Female,
                Picture = dto.Picture,
                Country = country,
                Data = new PlayerData
                {
                    Rank = dto.Data.Rank,
                    Points = dto.Data.Points,
                    Weight = dto.Data.Weight,
                    Height = dto.Data.Height,
                    Age = dto.Data.Age,
                    LastResults = dto.Data.Last
                        .Select((result, index) => new PlayerLastResult
                        {
                            Order = index,
                            Result = result == 1
                        })
                        .ToList()
                }
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return _helper.MapToPlayerDto(player);
        }
    }
}
