using Atelier.Api._Entities;
using Atelier.Api._Options;
using Atelier.Api.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Atelier.Api.Tests.Services
{
    [TestClass]
    public class JwtServiceTest
    {
        private JwtOption _opt = null!;
        private JwtService _service = null!;

        [TestInitialize]
        public void SetUp()
        {
            _opt = new JwtOption
            {
                Key = "super-secret-key-that-is-long-enough-for-hmac256!",
                Issuer = "test-issuer",
                Audience = "test-audience",
                ExpiryMinutes = 60
            };

            var optionsMock = new Mock<IOptions<JwtOption>>();
            optionsMock.Setup(o => o.Value).Returns(_opt);

            _service = new JwtService(optionsMock.Object);
        }

        #region GenerateJwtToken Tests

        [TestMethod]
        public void GenerateJwtToken_ReturnsNonEmptyTokenString()
        {
            // Act
            var (token, _) = _service.GenerateJwtToken(CreateUser());

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
        }

        [TestMethod]
        public void GenerateJwtToken_ReturnedStringIsValidJwt()
        {
            // Act
            var (token, _) = _service.GenerateJwtToken(CreateUser());

            // Assert
            Assert.AreEqual(3, token.Split('.').Length);
        }

        public void GenerateJwtToken_ExpirationIsInTheFuture()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var (_, expiration) = _service.GenerateJwtToken(CreateUser());

            // Assert
            Assert.IsTrue(expiration > before);
        }

        [TestMethod]
        public void GenerateJwtToken_ExpirationMatchesConfiguredMinutes()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var (_, expiration) = _service.GenerateJwtToken(CreateUser());
            
            // Assert
            var after = DateTime.UtcNow;
            var minExpected = before.AddMinutes(_opt.ExpiryMinutes);
            var maxExpected = after.AddMinutes(_opt.ExpiryMinutes);

            Assert.IsTrue(expiration >= minExpected && expiration <= maxExpected);
        }

        [TestMethod]
        public void GenerateJwtToken_TokenExpiryClaimMatchesReturnedExpiration()
        {
            // Act
            var (token, expiration) = _service.GenerateJwtToken(CreateUser());

            // Assert
            var jwt = DecodeToken(token);

            Assert.AreEqual(
                expiration.ToString("yyyy-MM-dd HH:mm:ss"),
                jwt.ValidTo.ToString("yyyy-MM-dd HH:mm:ss")
            );
        }

        [TestMethod]
        public void GenerateJwtToken_SubClaim_ContainsUserId()
        {
            // Arrange
            var user = CreateUser(id: 42);

            // Arrange
            var (token, _) = _service.GenerateJwtToken(user);

            // Assert
            var jwt = DecodeToken(token);

            var sub = jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
            Assert.AreEqual("42", sub);
        }

        [TestMethod]
        public void GenerateJwtToken_UniqueNameClaim_ContainsUsername()
        {
            // Arrange
            var user = CreateUser(username: "janedoe");

            // Act
            var (token, _) = _service.GenerateJwtToken(user);

            // Asseryt
            var jwt = DecodeToken(token);

            var uniqueName = jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value;
            Assert.AreEqual("janedoe", uniqueName);
        }

        [TestMethod]
        public void GenerateJwtToken_RoleClaim_ContainsUserRole()
        {
            // Arrange
            var user = CreateUser(role: "Player");

            // Act
            var (token, _) = _service.GenerateJwtToken(user);

            // Assert
            var jwt = DecodeToken(token);

            var role = jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            Assert.AreEqual("Player", role);
        }

        [TestMethod]
        public void GenerateJwtToken_JtiClaim_IsNonEmptyGuid()
        {
            // Act
            var (token, _) = _service.GenerateJwtToken(CreateUser());

            // Assert
            var jwt = DecodeToken(token);

            var jti = jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

            Assert.IsTrue(Guid.TryParse(jti, out _), $"jti '{jti}' is not a valid GUID");
        }

        [TestMethod]
        public void GenerateJwtToken_TwoCallsProduceDifferentJti()
        {
            // Arrange 
            var user = CreateUser();

            // Act
            var (tokenA, _) = _service.GenerateJwtToken(user);
            var (tokenB, _) = _service.GenerateJwtToken(user);

            // Assert
            var jtiA = DecodeToken(tokenA).Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            var jtiB = DecodeToken(tokenB).Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

            Assert.AreNotEqual(jtiA, jtiB);
        }

        [TestMethod]
        public void GenerateJwtToken_Issuer_MatchesConfiguration()
        {
            // Act 
            var (token, _) = _service.GenerateJwtToken(CreateUser());

            // Asert
            var jwt = DecodeToken(token);

            Assert.AreEqual(_opt.Issuer, jwt.Issuer);
        }

        [TestMethod]
        public void GenerateJwtToken_Audience_MatchesConfiguration()
        {
            // Act
            var (token, _) = _service.GenerateJwtToken(CreateUser());

            // Assrt
            var jwt = DecodeToken(token);

            Assert.AreEqual(_opt.Audience, jwt.Audiences.First());
        }

        [TestMethod]
        public void GenerateJwtToken_TokenIsValidWithCorrectKey()
        {
            // Act
            var (token, _) = _service.GenerateJwtToken(CreateUser());

            // Assert
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _opt.Issuer,
                ValidateAudience = true,
                ValidAudience = _opt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_opt.Key)
                ),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var handler = new JwtSecurityTokenHandler();
            
            handler.ValidateToken(token, parameters, out _);
        }

        [TestMethod]
        public void GenerateJwtToken_TokenIsInvalidWithWrongKey()
        {
            var (token, _) = _service.GenerateJwtToken(CreateUser());

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("wrong-key-that-wont-match-at-all!!")
                ),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            var handler = new JwtSecurityTokenHandler();
            Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(
                () => handler.ValidateToken(token, parameters, out _)
            );
        }

        #endregion

        #region Tests Helper
        private User CreateUser(int id = 1, string username = "johndoe", string role = "Admin")
            => new User { Id = id, Username = username, Role = role };

        private JwtSecurityToken DecodeToken(string token)
            => new JwtSecurityTokenHandler().ReadJwtToken(token);

        #endregion
    }
}
