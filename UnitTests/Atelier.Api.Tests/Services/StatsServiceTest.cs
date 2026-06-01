using Atelier.Api._Data;
using Atelier.Api._DTOs;
using Atelier.Api._Entities;
using Atelier.Api._Exception;
using Atelier.Api.Calculator;
using Atelier.Api.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atelier.Api.Tests.Services
{
    [TestClass]
    public class StatsServiceTest
    {
        private AppDbContext _context = null!;
        private Mock<IStatsCalculator> _calculatorMock = null!;
        private StatsService _service = null!;

        [TestInitialize]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _calculatorMock = new Mock<IStatsCalculator>();
            _service = new StatsService(_context, _calculatorMock.Object);
        }

        [TestCleanup]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region 

        [TestMethod]
        public async Task GetStatsAsync_EmptyDatabase_ThrowsNoDataException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NoDataException>(
                () => _service.GetStatsAsync()
            );
        }

        [TestMethod]
        public async Task GetStatsAsync_WithPlayers_ReturnsStatsDto()
        {
            // Arrange
            SeedPlayers(CreatePlayer(1), CreatePlayer(2));

            _calculatorMock.Setup(c => c.GetBestCountryAsync(It.IsAny<List<Player>>())).ReturnsAsync("FR");
            _calculatorMock.Setup(c => c.GetAverageBmiAsync(It.IsAny<List<Player>>())).ReturnsAsync(22.5);
            _calculatorMock.Setup(c => c.GetMedianHeightAsync(It.IsAny<List<Player>>())).ReturnsAsync(180);

            // Act
            var result = await _service.GetStatsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(StatsDto));
            _calculatorMock.Verify(c => c.GetBestCountryAsync(It.IsAny<List<Player>>()), Times.Once);
            _calculatorMock.Verify(c => c.GetAverageBmiAsync(It.IsAny<List<Player>>()), Times.Once);
            _calculatorMock.Verify(c => c.GetMedianHeightAsync(It.IsAny<List<Player>>()), Times.Once);
            Assert.AreEqual("FR", result.BestCountry);
            Assert.AreEqual(22.5, result.AverageBmi);
            Assert.AreEqual(180, result.MedianHeight);
        }

        [TestMethod]
        public async Task GetStatsAsync_EmptyDatabase_CalculatorIsNeverCalled()
        {
            // Act
            try { await _service.GetStatsAsync(); } catch (NoDataException) { }

            // Assert
            _calculatorMock.Verify(c => c.GetBestCountryAsync(It.IsAny<List<Player>>()), Times.Never);
            _calculatorMock.Verify(c => c.GetAverageBmiAsync(It.IsAny<List<Player>>()), Times.Never);
            _calculatorMock.Verify(c => c.GetMedianHeightAsync(It.IsAny<List<Player>>()), Times.Never);
        }

        #endregion

        private Player CreatePlayer(int id, string countryCode = "FR")
            => new Player
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                Sex = Sex.Male,
                Picture = string.Empty,
                Country = new Country { Code = countryCode, Picture = string.Empty },
                Data = new PlayerData
                {
                    Rank = id,
                    Points = 1000,
                    Weight = 75000,
                    Height = 180,
                    Age = 25,
                    LastResults = new List<PlayerLastResult>
                    {
                        new PlayerLastResult { Order = 0, Result = true },
                        new PlayerLastResult { Order = 1, Result = false }
                    }
                }
            };

        private void SeedPlayers(params Player[] players)
        {
            _context.Players.AddRange(players);
            _context.SaveChanges();
        }
    }
}
