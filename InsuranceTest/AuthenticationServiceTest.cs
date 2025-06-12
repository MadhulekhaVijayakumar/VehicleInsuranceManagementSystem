using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Repositories;
using InsuranceAPI.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceTest
{
    public class AuthenticationServiceTests
    {
        private Mock<IRepository<string, User>> _mockUserRepository;
        private Mock<IRepository<int, Client>> _mockClientRepository;
        private Mock<IRepository<int, Admin>> _mockAdminRepository;
        private Mock<ITokenService> _mockTokenService;
        private AuthenticationService _authenticationService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IRepository<string, User>>();
            _mockClientRepository = new Mock<IRepository<int, Client>>();
            _mockAdminRepository = new Mock<IRepository<int, Admin>>();
            _mockTokenService = new Mock<ITokenService>();

            _authenticationService = new AuthenticationService(
                _mockUserRepository.Object,
                _mockClientRepository.Object,
                _mockAdminRepository.Object,
                _mockTokenService.Object
            );
        }

        [Test]
        public async Task Login_ShouldReturnToken_ForValidClientLogin()
        {
            // Arrange
            var loginRequest = new UserLoginRequest
            {
                Username = "client@example.com",
                Password = "clientPassword"
            };

            // Simulate user with valid password hash
            var user = new User
            {
                Username = "client@example.com",
                Role = "Client",
                HashKey = new byte[64],
                Password = ComputeHash("clientPassword", new byte[64]) // Password hash simulated
            };
            var client = new Client { Id = 1, Name = "John Client", Email = "client@example.com" };

            _mockUserRepository.Setup(repo => repo.GetById(loginRequest.Username)).ReturnsAsync(user);
            _mockClientRepository.Setup(repo => repo.GetAll()).ReturnsAsync(new List<Client> { client });
            _mockTokenService.Setup(ts => ts.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("mockToken");

            // Act
            var result = await _authenticationService.Login(loginRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John Client", result.Name);
            Assert.AreEqual("Client", result.Role);
            Assert.AreEqual("mockToken", result.Token);
        }

        [Test]
        public async Task Login_ShouldReturnToken_ForValidAdminLogin()
        {
            // Arrange
            var loginRequest = new UserLoginRequest
            {
                Username = "admin@example.com",
                Password = "adminPassword"
            };

            // Simulate user with valid password hash
            var user = new User
            {
                Username = "admin@example.com",
                Role = "Admin",
                HashKey = new byte[64],
                Password = ComputeHash("adminPassword", new byte[64]) // Password hash simulated
            };
            var admin = new Admin { Id = 1, Name = "Admin User", Email = "admin@example.com" };

            _mockUserRepository.Setup(repo => repo.GetById(loginRequest.Username)).ReturnsAsync(user);
            _mockAdminRepository.Setup(repo => repo.GetAll()).ReturnsAsync(new List<Admin> { admin });
            _mockTokenService.Setup(ts => ts.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("mockToken");

            // Act
            var result = await _authenticationService.Login(loginRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Admin User", result.Name);
            Assert.AreEqual("Admin", result.Role);
            Assert.AreEqual("mockToken", result.Token);
        }

        [Test]
        public void Login_ShouldThrowUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var loginRequest = new UserLoginRequest
            {
                Username = "nonexistent@example.com",
                Password = "password"
            };

            _mockUserRepository.Setup(repo => repo.GetById(loginRequest.Username)).ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _authenticationService.Login(loginRequest));
        }

        [Test]
        public void Login_ShouldThrowUnauthorized_WhenPasswordIsInvalid()
        {
            // Arrange
            var loginRequest = new UserLoginRequest
            {
                Username = "client@example.com",
                Password = "wrongPassword"
            };

            var user = new User
            {
                Username = "client@example.com",
                Role = "Client",
                HashKey = new byte[64],
                Password = ComputeHash("clientPassword", new byte[64]) // Correct hash for "clientPassword"
            };

            _mockUserRepository.Setup(repo => repo.GetById(loginRequest.Username)).ReturnsAsync(user);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _authenticationService.Login(loginRequest));
        }

        [Test]
        public void Login_ShouldThrowUnauthorized_WhenRoleIsInvalid()
        {
            // Arrange
            var loginRequest = new UserLoginRequest
            {
                Username = "client@example.com",
                Password = "clientPassword"
            };

            var user = new User
            {
                Username = "client@example.com",
                Role = "InvalidRole",
                HashKey = new byte[64],
                Password = ComputeHash("clientPassword", new byte[64]) // Correct hash for "clientPassword"
            };

            _mockUserRepository.Setup(repo => repo.GetById(loginRequest.Username)).ReturnsAsync(user);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _authenticationService.Login(loginRequest));
        }

        // Helper method to simulate password hash computation
        private byte[] ComputeHash(string password, byte[] key)
        {
            using (var hmac = new HMACSHA512(key))
            {
                return hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
