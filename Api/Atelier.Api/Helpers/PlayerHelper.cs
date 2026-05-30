using Atelier.Api._DTOs;
using Atelier.Api._Entities;

namespace Atelier.Api.Helpers
{
    public interface IPlayerHelper
    {
        PlayerDto MapToPlayerDto(Player p);
    }

    public class PlayerHelper : IPlayerHelper
    {
        public PlayerDto MapToPlayerDto(Player p)
        {
            return new PlayerDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                ShortName = $"{p.FirstName[..1].ToUpper()}.{p.LastName[..Math.Min(3, p.LastName.Length)].ToUpper()}",
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
