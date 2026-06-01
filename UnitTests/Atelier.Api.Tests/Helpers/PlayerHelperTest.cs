using Atelier.Api._DTOs;
using Atelier.Api._Entities;
using Atelier.Api.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Atelier.Api.Tests.Helpers
{
    [TestClass]
    public class PlayerHelperTest
    {
        private PlayerHelper _helper;

        [TestInitialize]
        public void Setup()
        {
            _helper = new PlayerHelper();
        }

        [TestMethod]
        public void MapToPlayerDto_ValidPlayer_ReturnsCorrectDto()
        {
            // Arrange
            var country = CreateCountry();
            var data = CreatePlayerData();
            var player = CreatePlayer(country, data);
            

            // Act
            var dto = _helper.MapToPlayerDto(player);

            // Assert
            Assert.IsInstanceOfType(dto, typeof(PlayerDto));
        }


        private Country CreateCountry()
        {
            return new Country
            {
                Id = 1,
                Code = "US",
                Picture = "http://flag.com"
            };
        }

        private PlayerData CreatePlayerData()
        {
            return new PlayerData
            {
                Id = 1,
                Rank = 10,
                Points = 1000,
                Weight = 80000,
                Height = 190,
                Age = 30,
                LastResults = new List<PlayerLastResult>
                {
                    new PlayerLastResult { Id = 1, Order = 1, Result = true },
                    new PlayerLastResult { Id = 2, Order = 2, Result = false },
                    new PlayerLastResult { Id = 3, Order = 3, Result = true },
                    new PlayerLastResult { Id = 4, Order = 4, Result = true },
                    new PlayerLastResult { Id = 5, Order = 5, Result = true }
                }
            };
        }

        private Player CreatePlayer(Country country, PlayerData data)
        {
            return new Player
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Sex = Sex.Male,
                Picture = "http://photo.com",
                Country = country,
                Data = data
            };
        }
    }
}
