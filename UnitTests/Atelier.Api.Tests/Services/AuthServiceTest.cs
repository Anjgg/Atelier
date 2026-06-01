using Atelier.Api._Data;
using Atelier.Api._DTOs;
using Atelier.Api._Entities;
using Atelier.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atelier.Api.Tests.Services
{
    [TestClass]
    public class AuthServiceTest
    {
        private AppDbContext _context = null!;
        private Mock<IJwtService> _jwtServiceMock = null!;
        private Mock<IPasswordHasher<User>> _passwordHasherMock = null!;
        private AuthService _service = null!;

        [TestInitialize]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _jwtServiceMock = new Mock<IJwtService>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _service = new AuthService(_context, _jwtServiceMock.Object, _passwordHasherMock.Object);
        }

        [TestCleanup]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region LoginAsync Tests

        [TestMethod]
        public async Task LoginAsync_UnknownUsername_ReturnsNull()
        {
            // Arrange — empty DB
            var dto = CreateDto(username: "unknown");

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            Assert.IsNull(result);
            _jwtServiceMock.Verify(j => j.GenerateJwtToken(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public async Task LoginAsync_WrongPassword_ReturnsNull()
        {
            // Arrange
            var user = CreateUser();
            SeedUser(user);

            _passwordHasherMock
                .Setup(p => p.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(PasswordVerificationResult.Failed);

            // Act
            var result = await _service.LoginAsync(CreateDto());

            // Assert
            Assert.IsNull(result);
            _jwtServiceMock.Verify(j => j.GenerateJwtToken(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public async Task LoginAsync_ValidCredentials_ReturnsTokenAndExpiration()
        {
            // Arrange
            var user = CreateUser();
            var expected = ("jwt-token", DateTime.UtcNow.AddHours(1));
            SeedUser(user);

            _passwordHasherMock
                .Setup(p => p.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(PasswordVerificationResult.Success);

            _jwtServiceMock
                .Setup(j => j.GenerateJwtToken(It.IsAny<User>()))
                .Returns(expected);

            // Act
            var result = await _service.LoginAsync(CreateDto());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Item1, result!.Value.token);
            Assert.AreEqual(expected.Item2, result!.Value.expiration);
            _jwtServiceMock.Verify(j => j.GenerateJwtToken(It.IsAny<User>()), Times.Once);
        }

        #endregion

        #region Tests Helper

        private User CreateUser(string username = "johndoe", string passwordHash = "hashed-password", string role = "Admin")
            => new User { Id = 1, Username = username, PasswordHash = passwordHash, Role = role };

        private LoginRequestDto CreateDto(string username = "johndoe", string password = "plain-password")
            => new LoginRequestDto { Username = username, Password = password };

        private void SeedUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        #endregion
    }
}
