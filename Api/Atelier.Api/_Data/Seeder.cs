using Atelier.Api._Entities;
using Microsoft.AspNetCore.Identity;

namespace Atelier.Api._Data
{
    public class Seeder
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Users.Any())
            {
                var hasher = new PasswordHasher<User>();
                var admin = new User
                {
                    Username = "admin",
                    Role = "Admin"
                };
                admin.PasswordHash = hasher.HashPassword(admin, "admin123!");
                context.Users.Add(admin);

                var user = new User
                {
                    Username = "user",
                    Role = "User"
                };
                user.PasswordHash = hasher.HashPassword(user, "user123!");
                context.Users.Add(user);
                context.SaveChanges();
            }

            if (context.Players.Any()) return; // déjà seedé

            var serbia = new Country { Code = "SRB", Picture = "https://tenisu.latelier.co/resources/Serbie.png" };
            var usa = new Country { Code = "USA", Picture = "https://tenisu.latelier.co/resources/USA.png" };
            var sui = new Country { Code = "SUI", Picture = "https://tenisu.latelier.co/resources/Suisse.png" };
            var esp = new Country { Code = "ESP", Picture = "https://tenisu.latelier.co/resources/Espagne.png" };

            context.Countries.AddRange(serbia, usa, sui, esp);
            context.SaveChanges();

            var players = new List<Player>
            {
                new Player
                {
                    Id = 52,
                    FirstName = "Novak", LastName = "Djokovic",
                    ShortName = "N.DJO", Sex = Sex.Male,
                    Picture = "https://tenisu.latelier.co/resources/Djokovic.png",
                    CountryId = serbia.Id,
                    Data = new PlayerData
                    {
                        Rank = 2, Points = 2542, Weight = 80000, Height = 188, Age = 31,
                        LastResults = new List<PlayerLastResult>
                        {
                            new PlayerLastResult { Order = 0, Result = true },
                            new PlayerLastResult { Order = 1, Result = true },
                            new PlayerLastResult { Order = 2, Result = true },
                            new PlayerLastResult { Order = 3, Result = true },
                            new PlayerLastResult { Order = 4, Result = true }
                        }
                    }
                },
                new Player
                {
                    Id = 95,
                    FirstName = "Venus", LastName = "Williams",
                    ShortName = "V.WIL", Sex = Sex.Female,
                    Picture = "https://tenisu.latelier.co/resources/Venus.webp",
                    CountryId = usa.Id,
                    Data = new PlayerData
                    {
                        Rank = 52, Points = 1105, Weight = 74000, Height = 185, Age = 38,
                        LastResults = new List<PlayerLastResult>
                        {
                            new PlayerLastResult { Order = 0, Result = false },
                            new PlayerLastResult { Order = 1, Result = true },
                            new PlayerLastResult { Order = 2, Result = false },
                            new PlayerLastResult { Order = 3, Result = false },
                            new PlayerLastResult { Order = 4, Result = true }
                        }
                    }
                },
                new Player
                {
                    Id = 65,
                    FirstName = "Stan", LastName = "Wawrinka",
                    ShortName = "S.WAW", Sex = Sex.Male,
                    Picture = "https://tenisu.latelier.co/resources/Wawrinka.png",
                    CountryId = sui.Id,
                    Data = new PlayerData
                    {
                        Rank = 21, Points = 1784, Weight = 81000, Height = 183, Age = 33,
                        LastResults = new List<PlayerLastResult>
                        {
                            new PlayerLastResult { Order = 0, Result = true },
                            new PlayerLastResult { Order = 1, Result = true },
                            new PlayerLastResult { Order = 2, Result = true },
                            new PlayerLastResult { Order = 3, Result = false },
                            new PlayerLastResult { Order = 4, Result = true }
                        }
                    }
                },
                new Player
                {
                    Id = 102,
                    FirstName = "Serena", LastName = "Williams",
                    ShortName = "S.WIL", Sex = Sex.Female,
                    Picture = "https://tenisu.latelier.co/resources/Serena.png",
                    CountryId = usa.Id,
                    Data = new PlayerData
                    {
                        Rank = 10, Points = 3521, Weight = 72000, Height = 175, Age = 37,
                        LastResults = new List<PlayerLastResult>
                        {
                            new PlayerLastResult { Order = 0, Result = false },
                            new PlayerLastResult { Order = 1, Result = true },
                            new PlayerLastResult { Order = 2, Result = true },
                            new PlayerLastResult { Order = 3, Result = true },
                            new PlayerLastResult { Order = 4, Result = false }
                        }
                    }
                },
                new Player
                {
                    Id = 17,
                    FirstName = "Rafael", LastName = "Nadal",
                    ShortName = "R.NAD", Sex = Sex.Male,
                    Picture = "https://tenisu.latelier.co/resources/Nadal.png",
                    CountryId = esp.Id,
                    Data = new PlayerData
                    {
                        Rank = 1, Points = 1982, Weight = 85000, Height = 185, Age = 33,
                        LastResults = new List<PlayerLastResult>
                        {
                            new PlayerLastResult { Order = 0, Result = true },
                            new PlayerLastResult { Order = 1, Result = false },
                            new PlayerLastResult { Order = 2, Result = false },
                            new PlayerLastResult { Order = 3, Result = false },
                            new PlayerLastResult { Order = 4, Result = true }
                        }
                    }
                }
            };

            context.Players.AddRange(players);
            context.SaveChanges();
        }
    }
}
