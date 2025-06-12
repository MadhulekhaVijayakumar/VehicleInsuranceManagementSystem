using InsuranceAPI.Context;
using InsuranceAPI.Models;
using InsuranceAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests.Repositories
{
    [TestFixture]
    public class InsuranceRepositoryTests
    {
        private InsuranceManagementContext _context;
        private InsuranceRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new InsuranceManagementContext(options);

            // Seed client
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

            // Seed insurance
            var insurance = new Insurance
            {
                InsurancePolicyNumber = "POL500123",
                ProposalId = 1,
                VehicleId = 1,
                ClientId = 1,
                PremiumAmount = 3500,
                InsuranceSum = 75000,
                InsuranceStartDate = DateTime.Today.AddDays(-10),
                Status = "active",
                CreatedAt = DateTime.Now,
                Client = client
            };
            _context.Insurances.Add(insurance);

            _context.SaveChanges();

            _repository = new InsuranceRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetById_ShouldReturnInsurance_WhenPolicyExists()
        {
            var result = await _repository.GetById("POL500123");

            Assert.IsNotNull(result);
            Assert.AreEqual("POL500123", result.InsurancePolicyNumber);
            Assert.AreEqual(3500, result.PremiumAmount);
            Assert.IsNotNull(result.Client);
            Assert.AreEqual("Karthi", result.Client!.Name);
        }

        [Test]
        public void GetById_ShouldThrowException_WhenPolicyNotFound()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetById("NONEXISTENT"));
            Assert.AreEqual("Insurance with ID NONEXISTENT not present", ex.Message);
        }

        [Test]
        public async Task GetAll_ShouldReturnAllInsurances()
        {
            var result = await _repository.GetAll();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
            Assert.AreEqual("Karthi", result.First().Client!.Name);
        }

        [Test]
        public async Task GetAll_ShouldThrowException_WhenNoInsurancesExist()
        {
            // Remove all and save
            _context.Insurances.RemoveRange(_context.Insurances);
            await _context.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetAll());
            Assert.AreEqual("No insurances found", ex.Message);
        }
    }
}
