using InsuranceAPI.Context;
using InsuranceAPI.Models;
using InsuranceAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace InsuranceAPI.Tests.Repositories
{
    [TestFixture]
    public class VehicleRepositoryTests
    {
        private InsuranceManagementContext _context;
        private VehicleRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // ensures fresh DB per test run
                .Options;

            _context = new InsuranceManagementContext(options);

            // Add a client
            var client = new Client
            {
                Id = 1,
                Name = "Karthi",
                DateOfBirth = new DateTime(1999, 1, 1),
                Gender = "Male",
                PhoneNumber = "9999999999",
                Email = "karthi@gmail.com",
                AadhaarNumber = "123412341234",
                PANNumber = "ABCDE1234F",
                Address = "Chennimalai"
            };
            _context.Clients.Add(client);

            // Add a vehicle
            var vehicle = new Vehicle
            {
                VehicleId = 1,
                ClientId = 1,
                VehicleType = "Car",
                VehicleNumber = "TN01AB1234",
                ChassisNumber = "CH123456789",
                EngineNumber = "EN987654321",
                MakerName = "Hyundai",
                ModelName = "i20",
                VehicleColor = "Blue",
                FuelType = "Petrol",
                RegistrationDate = DateTime.Today,
                SeatCapacity = 5
            };
            _context.Vehicles.Add(vehicle);

            _context.SaveChanges();

            _repository = new VehicleRepository(_context);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetById_ShouldReturnVehicle_WhenIdExists()
        {
            var result = await _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Hyundai", result.MakerName);
            Assert.AreEqual("i20", result.ModelName);
            Assert.AreEqual("Blue", result.VehicleColor);
        }

        [Test]
        public async Task GetAll_ShouldReturnAllVehicles()
        {
            var result = await _repository.GetAll();

            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty((System.Collections.ICollection)result);
        }

        [Test]
        public void GetById_ShouldThrowException_WhenVehicleNotFound()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetById(99));
            Assert.AreEqual("Vehicle with ID 99 not found", ex.Message);
        }
    }
}