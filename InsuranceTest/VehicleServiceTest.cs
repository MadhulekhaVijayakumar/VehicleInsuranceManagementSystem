using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System.Security.Claims;

namespace InsuranceAPI.Tests.Services
{
    public class VehicleServiceTests
    {
        private Mock<IRepository<int, Vehicle>> _vehicleRepoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private VehicleService _vehicleService;

        private readonly int _mockClientId = 101;

        [SetUp]
        public void Setup()
        {
            _vehicleRepoMock = new Mock<IRepository<int, Vehicle>>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _mockClientId.ToString())
            }, "mock"));

            var httpContext = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);

            _vehicleService = new VehicleService(_vehicleRepoMock.Object, _httpContextAccessorMock.Object);
        }

        [Test]
        public async Task RegisterVehicle_ShouldReturnCreatedVehicleId()
        {
            // Arrange
            var request = new CreateVehicleRequest
            {
                VehicleType = "Car",
                VehicleNumber = "TN01AB1234",
                ChassisNumber = "CHS12345678",
                EngineNumber = "ENG12345678",
                MakerName = "Toyota",
                ModelName = "Innova",
                VehicleColor = "White",
                FuelType = "Petrol",
                RegistrationDate = new DateTime(2020, 1, 1),
                SeatCapacity = 5
            };

            var expectedVehicle = new Vehicle
            {
                VehicleId = 1,
                ClientId = _mockClientId,
                VehicleType = request.VehicleType,
                VehicleNumber = request.VehicleNumber,
                ChassisNumber = request.ChassisNumber,
                EngineNumber = request.EngineNumber,
                MakerName = request.MakerName,
                ModelName = request.ModelName,
                VehicleColor = request.VehicleColor,
                FuelType = request.FuelType,
                RegistrationDate = request.RegistrationDate,
                SeatCapacity = request.SeatCapacity
            };

            _vehicleRepoMock.Setup(r => r.Add(It.IsAny<Vehicle>())).ReturnsAsync(expectedVehicle);

            // Act
            var result = await _vehicleService.RegisterVehicle(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.VehicleId, Is.EqualTo(expectedVehicle.VehicleId));
            Assert.That(result.Message, Is.EqualTo("Vehicle registered successfully"));
        }

        [Test]
        public async Task GetAllVehiclesByClient_ShouldReturnOnlyClientVehicles()
        {
            // Arrange
            var allVehicles = new List<Vehicle>
            {
                new Vehicle { VehicleId = 1, ClientId = _mockClientId, VehicleType = "Car", VehicleNumber = "TN01X1111", ChassisNumber = "CH123", EngineNumber = "EN123", MakerName = "Toyota", ModelName = "Etios", VehicleColor = "Red", FuelType = "Petrol", RegistrationDate = DateTime.Now, SeatCapacity = 5 },
                new Vehicle { VehicleId = 2, ClientId = 999, VehicleType = "Bike", VehicleNumber = "TN02Y2222", ChassisNumber = "CH999", EngineNumber = "EN999", MakerName = "Honda", ModelName = "Unicorn", VehicleColor = "Black", FuelType = "Petrol", RegistrationDate = DateTime.Now, SeatCapacity = 2 }
            };

            _vehicleRepoMock.Setup(r => r.GetAll()).ReturnsAsync(allVehicles);

            // Act
            var result = await _vehicleService.GetAllVehiclesByClient(_mockClientId);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().VehicleNumber, Is.EqualTo("TN01X1111"));
        }

        [Test]
        public async Task GetVehicleById_ShouldReturnVehicleDto()
        {
            // Arrange
            var vehicleId = 1;
            var vehicle = new Vehicle
            {
                VehicleId = vehicleId,
                ClientId = _mockClientId,
                VehicleType = "Bike",
                VehicleNumber = "TN03Z3333",
                ChassisNumber = "CH333",
                EngineNumber = "EN333",
                MakerName = "Bajaj",
                ModelName = "Pulsar",
                VehicleColor = "Blue",
                FuelType = "Petrol",
                RegistrationDate = DateTime.Today,
                SeatCapacity = 2
            };

            _vehicleRepoMock.Setup(r => r.GetById(vehicleId)).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleService.GetVehicleById(vehicleId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.VehicleId, Is.EqualTo(vehicleId));
            Assert.That(result.VehicleNumber, Is.EqualTo("TN03Z3333"));
        }
    }
}
