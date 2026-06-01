using Atelier.Api._Data;
using Atelier.Api._DTOs;
using Atelier.Api._Entities;
using Atelier.Api._Exception;
using Atelier.Api.Helpers;
using Atelier.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Atelier.Api.Tests.Services
{
    [TestClass]
    public class PlayerServiceTest
    {
        private AppDbContext _context = null!;
        private Mock<IPlayerHelper> _helperMock = null!;
        private PlayerService _service = null!;

        [TestInitialize]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _helperMock = new Mock<IPlayerHelper>();
            _service = new PlayerService(_context, _helperMock.Object);
        }

        [TestCleanup]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetAllPlayersAsync Tests

        [TestMethod]
        public async Task GetAllPlayersAsync_WithMixedPlayers_ReturnsSeparatedByGender()
        {
            // Arrange
            _context.Players.AddRange(
                CreatePlayer(1, "Roger", "Federer", Sex.Male, 1),
                CreatePlayer(2, "Serena", "Williams", Sex.Female, 2),
                CreatePlayer(3, "Rafael", "Nadal", Sex.Male, 3)
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllPlayersAsync();

            // Assert
            Assert.AreEqual(2, result.Male.Count);
            Assert.AreEqual(1, result.Female.Count);
            Assert.IsTrue(result.Male.Any(p => p.Name == "Roger Federer"));
            Assert.IsTrue(result.Female.Any(p => p.Name == "Serena Williams"));
        }

        [TestMethod]
        public async Task GetAllPlayersAsync_OrdersByRank()
        {
            // Arrange
            _context.Players.AddRange(
                CreatePlayer(1, "B", "Player", Sex.Male, 3),
                CreatePlayer(2, "A", "Player", Sex.Male, 1),
                CreatePlayer(3, "C", "Player", Sex.Male, 2)
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllPlayersAsync();

            // Assert – male list should follow rank order (1 → 2 → 3)
            var ids = result.Male.Select(p => p.Id).ToList();
            CollectionAssert.AreEqual(new[] { 2, 3, 1 }, ids);
        }

        [TestMethod]
        public async Task GetAllPlayersAsync_NoBothGenders_ReturnsEmptyListForMissingSex()
        {
            // Arrange – only male players
            _context.Players.Add(CreatePlayer(1, "Roger", "Federer", Sex.Male, 1));
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllPlayersAsync();

            // Assert
            Assert.AreEqual(1, result.Male.Count);
            Assert.AreEqual(0, result.Female.Count);
        }

        [TestMethod]
        public async Task GetAllPlayersAsync_EmptyDatabase_ThrowsNoDataException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NoDataException>(
                () => _service.GetAllPlayersAsync()
            );
        }

        [TestMethod]
        public async Task GetAllPlayersAsync_PlayerNameFormat_IsFirstNameSpaceLastName()
        {
            // Arrange
            _context.Players.Add(CreatePlayer(1, "Rafael", "Nadal", Sex.Male, 1));
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllPlayersAsync();

            // Assert
            Assert.AreEqual("Rafael Nadal", result.Male.First().Name);
        }

        #endregion

        #region GetPlayerByIdAsync Tests

        [TestMethod]
        public async Task GetPlayerByIdAsync_ExistingId_ReturnsMappedDto()
        {
            // Arrange
            var player = CreatePlayer(1, "Roger", "Federer", Sex.Male, 1);
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            var expectedDto = MakeDto(player);
            _helperMock.Setup(h => h.MapToPlayerDto(It.IsAny<Player>())).Returns(expectedDto);

            // Act
            var result = await _service.GetPlayerByIdAsync(1);

            // Assert
            Assert.AreEqual(expectedDto.Id, result.Id);
            Assert.AreEqual(expectedDto.FirstName, result.FirstName);
            Assert.AreEqual(expectedDto.LastName, result.LastName);
        }

        [TestMethod]
        public async Task GetPlayerByIdAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.GetPlayerByIdAsync(999)
            );
        }

        [TestMethod]
        public async Task GetPlayerByIdAsync_CallsMapToPlayerDto_WithCorrectPlayer()
        {
            // Arrange
            var player = CreatePlayer(42, "Novak", "Djokovic", Sex.Male, 1);
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            _helperMock.Setup(h => h.MapToPlayerDto(It.IsAny<Player>())).Returns(MakeDto(player));

            // Act
            await _service.GetPlayerByIdAsync(42);

            // Assert
            _helperMock.Verify(
                h => h.MapToPlayerDto(It.Is<Player>(p => p.Id == 42)),
                Times.Once
            );
        }

        #endregion

        #region CreatePlayerAsync Tests

        [TestMethod]
        public async Task CreatePlayerAsync_ValidDto_PlayerIsSavedInDatabase()
        {
            // Arrange
            var dto = BuildCreateDto();
            _helperMock.Setup(h => h.MapToPlayerDto(It.IsAny<Player>())).Returns(new PlayerDto());

            // Act
            await _service.CreatePlayerAsync(dto);

            // Assert
            Assert.AreEqual(1, _context.Players.Count());
            var saved = _context.Players.First();
            Assert.AreEqual("Carlos", saved.FirstName);
            Assert.AreEqual("Alcaraz", saved.LastName);
        }

        [TestMethod]
        public async Task CreatePlayerAsync_SexM_SetsMaleSex()
        {
            // Arrange
            var dto = BuildCreateDto(sex: "M");
            _helperMock.Setup(h => h.MapToPlayerDto(It.IsAny<Player>())).Returns(new PlayerDto());

            // Act
            await _service.CreatePlayerAsync(dto);

            // Assert
            Assert.AreEqual(Sex.Male, _context.Players.First().Sex);
        }

        [TestMethod]
        public async Task CreatePlayerAsync_SexOtherThanM_SetsFemaleSex()
        {
            // Arrange
            var dto = BuildCreateDto(sex: "F");
            _helperMock.Setup(h => h.MapToPlayerDto(It.IsAny<Player>())).Returns(new PlayerDto());

            // Act
            await _service.CreatePlayerAsync(dto);

            // Assert
            Assert.AreEqual(Sex.Female, _context.Players.First().Sex);
        }

        [TestMethod]
        public async Task CreatePlayerAsync_ExistingCountry_ReusesCountry()
        {
            // Arrange
            var existing = new Country { Code = "ES", Picture = "flag.png" };
            _context.Countries.Add(existing);
            await _context.SaveChangesAsync();

            var dto = BuildCreateDto(countryCode: "ES");
            _helperMock.Setup(h => h.MapToPlayerDto(It.IsAny<Player>())).Returns(new PlayerDto());

            // Act
            await _service.CreatePlayerAsync(dto);

            // Assert – only one country in DB (the original one was reused)
            Assert.AreEqual(1, _context.Countries.Count());
            Assert.AreEqual("flag.png", _context.Countries.First().Picture);
        }

        [TestMethod]
        public async Task CreatePlayerAsync_UnknownCountry_CreatesNewCountry()
        {
            // Arrange
            var dto = BuildCreateDto(countryCode: "JP");
            _helperMock.Setup(h => h.MapToPlayerDto(It.IsAny<Player>())).Returns(new PlayerDto());

            // Act
            await _service.CreatePlayerAsync(dto);

            // Assert
            Assert.AreEqual(1, _context.Countries.Count(c => c.Code == "JP"));
        }

        [TestMethod]
        public async Task CreatePlayerAsync_LastResults_AreMappedCorrectly()
        {
            // Arrange
            var dto = BuildCreateDto(); // Last = [1, 0, 1, 1, 0]
            _helperMock.Setup(h => h.MapToPlayerDto(It.IsAny<Player>())).Returns(new PlayerDto());

            // Act
            await _service.CreatePlayerAsync(dto);

            // Assert
            var results = _context.Players
                .Include(p => p.Data).ThenInclude(d => d.LastResults)
                .First().Data.LastResults
                .OrderBy(r => r.Order)
                .ToList();

            Assert.AreEqual(5, results.Count);
            Assert.IsTrue(results[0].Result); // 1 → true
            Assert.IsFalse(results[1].Result); // 0 → false
            Assert.IsTrue(results[2].Result); // 1 → true
            Assert.AreEqual(0, results[0].Order);
            Assert.AreEqual(4, results[4].Order);
        }

        [TestMethod]
        public async Task CreatePlayerAsync_NullPicture_SetsEmptyString()
        {
            // Arrange
            var dto = BuildCreateDto();
            dto.Picture = null;
            _helperMock.Setup(h => h.MapToPlayerDto(It.IsAny<Player>())).Returns(new PlayerDto());

            // Act
            await _service.CreatePlayerAsync(dto);

            // Assert
            Assert.AreEqual(string.Empty, _context.Players.First().Picture);
        }

        [TestMethod]
        public async Task CreatePlayerAsync_ReturnsDto_FromHelper()
        {
            // Arrange
            var dto = BuildCreateDto();
            var expected = new PlayerDto { Id = 99, FirstName = "Carlos", LastName = "Alcaraz" };
            _helperMock.Setup(h => h.MapToPlayerDto(It.IsAny<Player>())).Returns(expected);

            // Act
            var result = await _service.CreatePlayerAsync(dto);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public async Task CreatePlayerAsync_CallsMapToPlayerDto_ExactlyOnce()
        {
            // Arrange
            var dto = BuildCreateDto();
            _helperMock.Setup(h => h.MapToPlayerDto(It.IsAny<Player>())).Returns(new PlayerDto());

            // Act
            await _service.CreatePlayerAsync(dto);

            // Assert
            _helperMock.Verify(h => h.MapToPlayerDto(It.IsAny<Player>()), Times.Once);
        }
    

        #endregion

        #region Tests Helpers

        private Player CreatePlayer(int id, string firstName, string lastName, Sex sex, int rank)
            => new Player
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Sex = sex,
                Picture = string.Empty,
                Country = new Country { Code = "FR", Picture = string.Empty },
                Data = new PlayerData { Rank = rank, Points = 100, Weight = 70000, Height = 180, Age = 25 }
            };

        private PlayerDto MakeDto(Player p)
            => new PlayerDto { Id = p.Id, FirstName = p.FirstName, LastName = p.LastName };

        private CreatePlayerDto BuildCreateDto(string countryCode = "FR", string sex = "M")
            => new CreatePlayerDto
            {
                FirstName = "Carlos",
                LastName = "Alcaraz",
                Sex = sex,
                Picture = "pic.jpg",
                CountryCode = countryCode,
                Data = new CreatePlayerDataDto
                {
                    Rank = 1,
                    Points = 9000,
                    Weight = 75000,
                    Height = 185,
                    Age = 21,
                    Last = new List<int> { 1, 0, 1, 1, 0 }
                }
            };

        #endregion

    }
}
