using InsuranceAPI.Services;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsuranceTest
{
    public class TokenServiceTests
    {
        private TokenService _tokenService;

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Keys:JwtToken", "This is a secret key for jwt token generation. This long is enough??"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenService = new TokenService(configuration);
        }

        [Test]
        public async Task GenerateToken_ShouldReturn_ValidJwtToken()
        {
            // Arrange
            int id = 1;
            string name = "Test User";
            string role = "Admin";
            string email = "test@example.com";

            // Act
            var token = await _tokenService.GenerateToken(id, name, role, email);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsNotEmpty(token);
            Assert.That(token.Split('.').Length, Is.EqualTo(3), "Token is not in JWT format.");
        }
    }
}
