using Atelier.Api._Entities;

namespace Atelier.Api.Calculator
{
    public interface IStatsCalculator
    {
        string GetBestCountry(List<Player> players);
        double GetAverageBmi(List<Player> players);
        double GetMedianHeight(List<Player> players);
    }
    public class StatsCalculator : IStatsCalculator
    {
        public string GetBestCountry(List<Player> players)
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

        public double GetAverageBmi(List<Player> players)
        {
            return Math.Round(players.Average(p =>
            {
                double weightKg = p.Data.Weight / 1000.0;
                double heightM = p.Data.Height / 100.0;
                return weightKg / (heightM * heightM);
            }), 2);
        }

        public double GetMedianHeight(List<Player> players)
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
    }
}
