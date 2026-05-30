using Atelier.Api._Data;
using Atelier.Api._DTOs;
using Atelier.Api._Exception;
using Atelier.Api.Calculator;
using Microsoft.EntityFrameworkCore;

namespace Atelier.Api.Services
{
    public interface IStatsService
    {
        Task<StatsDto?> GetStatsAsync();
    }

    public class StatsService : IStatsService
    {
        private readonly AppDbContext _context;
        private readonly IStatsCalculator _calculator;


        public StatsService (AppDbContext context, IStatsCalculator calculator)
        {
            _context = context;
            _calculator = calculator;
        }

        public async Task<StatsDto?> GetStatsAsync()
        {
            var players = await _context.Players
                .Include(p => p.Country)
                .Include(p => p.Data)
                .ThenInclude(p => p.LastResults)
                .ToListAsync();

            if (players == null || players.Count == 0)
            {
                throw new NoDataException("No data has been retrieved");
            }

            return new StatsDto
            {
                BestCountry = _calculator.GetBestCountry(players),
                AverageBmi = _calculator.GetAverageBmi(players),
                MedianHeight = _calculator.GetMedianHeight(players)
            };
        }
    }
}
