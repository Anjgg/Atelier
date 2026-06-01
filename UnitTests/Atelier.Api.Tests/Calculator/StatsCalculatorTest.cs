using Atelier.Api._Entities;
using Atelier.Api._Exception;
using Atelier.Api.Calculator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atelier.Api.Tests.Calculator
{
    [TestClass]
    public class StatsCalculatorTest
    {
        private IStatsCalculator _calculator = null!;

        [TestInitialize]
        public void SetUp() => _calculator = new StatsCalculator();

        #region GetBestCountryAsync Tests

        [TestMethod]
        public async Task GetBestCountryAsync_SingleCountry_ReturnsThatCountry()
        {
            var players = new List<Player>
            {
                CreatePlayer("FR", 75000, 180, true, false, true)
            };

            var result = await _calculator.GetBestCountryAsync(players);

            Assert.AreEqual("FR", result);
        }

        [TestMethod]
        public async Task GetBestCountryAsync_TwoCountries_ReturnsHigherWinRate()
        {
            var players = new List<Player>
            {
                CreatePlayer("ES", 75000, 180, true, true, true, true, true),
                CreatePlayer("FR", 75000, 180, true, false, false, false, false)
            };

            var result = await _calculator.GetBestCountryAsync(players);

            Assert.AreEqual("ES", result);
        }

        [TestMethod]
        public async Task GetBestCountryAsync_MultiplePlayersPerCountry_AveragesWinRate()
        {
            var players = new List<Player>
            {
                CreatePlayer("DE", 75000, 180, true, true, true, false, false),
                CreatePlayer("DE", 75000, 180, false, false, false, false, false),

                CreatePlayer("US", 75000, 180, true, true, false, true, true),
                CreatePlayer("US", 75000, 180, true, true, false, true, true)
            };

            var result = await _calculator.GetBestCountryAsync(players);

            Assert.AreEqual("US", result);
        }

        [TestMethod]
        public async Task GetBestCountryAsync_EmptyList_ThrowsStatsCalculatorException()
        {
            await Assert.ThrowsAsync<StatsCalculatorException>(
                () => _calculator.GetBestCountryAsync(new List<Player>())
            );
        }

        #endregion

        #region GetAverageBmiAsync Tests

        [TestMethod]
        public async Task GetAverageBmiAsync_SinglePlayer_ReturnsCorrectBmi()
        {
            var players = new List<Player>
            {
                CreatePlayer("FR", 70000, 175, true)
            };

            var result = await _calculator.GetAverageBmiAsync(players);

            Assert.AreEqual(22.86, result, 0.01);
        }

        [TestMethod]
        public async Task GetAverageBmiAsync_TwoPlayers_ReturnsAverageBmi()
        {
            var players = new List<Player>
            {
                CreatePlayer("FR", 80000, 180, true),
                CreatePlayer("FR", 60000, 170, true)
            };

            var result = await _calculator.GetAverageBmiAsync(players);

            Assert.AreEqual(22.73, result, 0.01);
        }

        [TestMethod]
        public async Task GetAverageBmiAsync_EmptyList_ThrowsStatsCalculatorException()
        {
            await Assert.ThrowsAsync<StatsCalculatorException>(
                () => _calculator.GetAverageBmiAsync(new List<Player>())
            );
        }

        #endregion

        #region GetMedianHeightAsync Tests

        [TestMethod]
        public async Task GetMedianHeightAsync_OddCount_ReturnsMiddleValue()
        {
            var players = new List<Player>
            {
                CreatePlayer("FR", 70000, 180, true),
                CreatePlayer("FR", 70000, 160, true),
                CreatePlayer("FR", 70000, 170, true)
            };

            var result = await _calculator.GetMedianHeightAsync(players);

            Assert.AreEqual(170.0, result);
        }

        #endregion

        private static Player CreatePlayer(
            string countryCode,
            int weight,
            int height,
            params bool[] results)
            => new Player
            {
                FirstName = "Test",
                LastName = "Player",
                Sex = Sex.Male,
                Picture = string.Empty,
                Country = new Country { Code = countryCode, Picture = string.Empty },
                Data = new PlayerData
                {
                    Rank = 1,
                    Points = 1000,
                    Weight = weight,
                    Height = height,
                    Age = 25,
                    LastResults = results
                        .Select((r, i) => new PlayerLastResult { Order = i, Result = r })
                        .ToList()
                }
            };
    }
}
