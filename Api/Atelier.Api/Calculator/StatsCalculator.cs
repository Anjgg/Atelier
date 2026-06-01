using Atelier.Api._Entities;
using Atelier.Api._Exception;

namespace Atelier.Api.Calculator
{
    public interface IStatsCalculator
    {
        Task<string> GetBestCountryAsync(List<Player> players);
        Task<double> GetAverageBmiAsync(List<Player> players);
        Task<double> GetMedianHeightAsync(List<Player> players);
    }
    public class StatsCalculator : IStatsCalculator
    {
        public async Task<string> GetBestCountryAsync(List<Player> players)
        {
            try
            {
                return players
                 .GroupBy(p => p.Country.Code)
                 .Select(g => new
                 {
                     Country = g.Key,
                     WinRate = g.Average(p =>
                         p.Data.LastResults.Count(r => r.Result) * 1.0 /
                         p.Data.LastResults.Count)
                 })
                 .OrderByDescending(x => x.WinRate)
                 .First().Country;
            }
            catch
            {
                throw new StatsCalculatorException("An exception was threw during compute the best country");
            }
        }

        public async Task<double> GetAverageBmiAsync(List<Player> players)
        {
            try
            {
                return Math.Round(players.Average(p =>
                    {
                        double weightKg = p.Data.Weight / 1000.0;
                        double heightM = p.Data.Height / 100.0;
                        return weightKg / (heightM * heightM);
                    }), 2);
            } 
            catch 
            {
                throw new StatsCalculatorException("An exception was threw during compute the average BMI");
            }
        }

        public async Task<double> GetMedianHeightAsync(List<Player> players)
        {
            try
            {
                var heights = players
                .Select(p => p.Data.Height)
                .OrderBy(h => h)
                .ToList();

                int count = heights.Count;
                if (count % 2 == 0)
                    return (heights[count / 2 - 1] + heights[count / 2]) / 2.0;
                else
                    return heights[count / 2];
            }
            catch
            {
                throw new StatsCalculatorException("An exception was threw during compute the median height");
            }
        }
    }
}
