using Moq;
using NUnit.Framework;
using InsuranceAPI.Services;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using InsuranceAPI.Interfaces;

namespace InsuranceTest
{
    [TestFixture]
    public class AdminServiceTests
    {
        private Mock<IRepository<string, User>> _mockUserRepository;
        private Mock<IRepository<int, Admin>> _mockAdminRepository;
        private AdminService _adminService;

        [SetUp]
        public void SetUp()
        {
            // Mock the repositories
            _mockUserRepository = new Mock<IRepository<string, User>>();
            _mockAdminRepository = new Mock<IRepository<int, Admin>>();

            // Initialize AdminService with the mocked repositories
            _adminService = new AdminService(_mockUserRepository.Object, _mockAdminRepository.Object);
        }

        [Test]
        public async Task CreateAdmin_ShouldCreateAdminAndReturnSuccessResponse()
        {
            // Arrange
            var request = new CreateAdminRequest
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Password = "password123"
            };

            // Mock the user repository to return a mock user
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).ReturnsAsync(new User
            {
                Username = request.Email,
                Role = "Admin",
                Password = new HMACSHA512().ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
                HashKey = new HMACSHA512().Key
            });

            // Mock the admin repository to return a mock admin
            _mockAdminRepository.Setup(repo => repo.Add(It.IsAny<Admin>())).ReturnsAsync(new Admin
            {
                Id = 1,
                Name = request.Name,
                Email = request.Email
            });

            // Act
            var result = await _adminService.CreateAdmin(request);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Admin created successfully", result.Message);
            Assert.AreEqual(1, result.Id);
        }

        [Test]
        public void CreateAdmin_ShouldThrowException_WhenUserCreationFails()
        {
            // Arrange
            var request = new CreateAdminRequest
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Password = "password123"
            };

            // Mock the user repository to simulate a failure
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _adminService.CreateAdmin(request));
            Assert.AreEqual("Failed to create user", ex.Message);
        }

        [Test]
        public void CreateAdmin_ShouldThrowException_WhenAdminCreationFails()
        {
            // Arrange
            var request = new CreateAdminRequest
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Password = "password123"
            };

            // Mock the user repository to return a user
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).ReturnsAsync(new User
            {
                Username = request.Email,
                Role = "Admin",
                Password = new HMACSHA512().ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
                HashKey = new HMACSHA512().Key
            });

            // Mock the admin repository to simulate a failure
            _mockAdminRepository.Setup(repo => repo.Add(It.IsAny<Admin>())).ReturnsAsync((Admin)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _adminService.CreateAdmin(request));
            Assert.AreEqual("Failed to create admin", ex.Message);
        }
    }
}
