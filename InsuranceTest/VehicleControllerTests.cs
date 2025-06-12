using InsuranceAPI.Controllers;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests.Controllers
{
    public class VehicleControllerTests
    {
        private Mock<IVehicleService> _mockVehicleService;
        private VehicleController _controller;

        [SetUp]
        public void Setup()
        {
            _mockVehicleService = new Mock<IVehicleService>();
            _controller = new VehicleController(_mockVehicleService.Object);
        }

        [Test]
        public async Task GetMyVehicles_ReturnsOkResult_WithVehicleList()
        {
            // Arrange
            var clientId = 1;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, clientId.ToString()),
                new Claim(ClaimTypes.Role, "Client")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };

            var vehicles = new List<VehicleDto>
            {
                new VehicleDto
                {
                    VehicleId = 1,
                    VechileType = "SUV",
                    VehicleNumber = "MH12AB1234",
                    ChassisNumber = "CH1234567890",
                    EngineNumber = "EN987654321",
                    MakerName = "Toyota",
                    ModelName = "Fortuner",
                    VehicleColor = "Black",
                    FuelType = "Diesel",
                    RegistrationDate = new DateTime(2021, 5, 20),
                    SeatCapacity = 7
                },
                new VehicleDto
                {
                    VehicleId = 2,
                    VechileType = "Hatchback",
                    VehicleNumber = "MH14XY5678",
                    ChassisNumber = "CH0987654321",
                    EngineNumber = "EN123456789",
                    MakerName = "Hyundai",
                    ModelName = "i20",
                    VehicleColor = "Red",
                    FuelType = "Petrol",
                    RegistrationDate = new DateTime(2020, 8, 15),
                    SeatCapacity = 5
                }
            };

            _mockVehicleService.Setup(s => s.GetAllVehiclesByClient(clientId))
                               .ReturnsAsync(vehicles);

            // Act
            var result = await _controller.GetMyVehicles();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<IEnumerable<VehicleDto>>(okResult.Value);

            var returnedVehicles = okResult.Value as List<VehicleDto>;
            Assert.AreEqual(2, returnedVehicles.Count);
            Assert.AreEqual("Fortuner", returnedVehicles[0].ModelName);
            Assert.AreEqual("i20", returnedVehicles[1].ModelName);
        }

        [Test]
        public async Task GetMyVehicles_ReturnsUnauthorized_WhenClientIdMissing()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity()) // No claims
                }
            };

            // Act
            var result = await _controller.GetMyVehicles();

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result.Result);

            var unauthorized = result.Result as UnauthorizedObjectResult;
            Assert.AreEqual("Client ID not found in token.", unauthorized.Value);
        }
    }
}
