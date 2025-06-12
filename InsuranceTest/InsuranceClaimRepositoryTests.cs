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
    public class InsuranceClaimRepositoryTests
    {
        private InsuranceManagementContext _context;
        private InsuranceClaimRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new InsuranceManagementContext(options);

            // Seed Insurance
            var insurance = new Insurance
            {
                InsurancePolicyNumber = "POL123456",
                ProposalId = 1,
                VehicleId = 1,
                ClientId = 1,
                PremiumAmount = 5000,
                InsuranceSum = 100000,
                InsuranceStartDate = DateTime.Today.AddMonths(-1),
                Status = "active",
                CreatedAt = DateTime.Now
            };
            _context.Insurances.Add(insurance);

            // Seed InsuranceClaim
            var claim = new InsuranceClaim
            {
                ClaimId = 1,
                InsurancePolicyNumber = "POL123456",
                IncidentDate = DateTime.Today.AddDays(-5),
                ClaimAmount = 15000,
                Description = "Minor accident",
                Status = "Pending",
                CreatedAt = DateTime.Now,
                Insurance = insurance
            };
            _context.InsuranceClaims.Add(claim);

            _context.SaveChanges();

            _repository = new InsuranceClaimRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetById_ShouldReturnClaim_WhenIdExists()
        {
            var result = await _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("POL123456", result.InsurancePolicyNumber);
            Assert.AreEqual("Minor accident", result.Description);
            Assert.IsNotNull(result.Insurance);
            Assert.AreEqual(100000, result.Insurance!.InsuranceSum);
        }

        [Test]
        public void GetById_ShouldThrowException_WhenClaimNotFound()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetById(99));
            Assert.AreEqual("Claim with ID 99 not found", ex.Message);
        }

        [Test]
        public async Task GetAll_ShouldReturnClaims_WhenClaimsExist()
        {
            var results = await _repository.GetAll();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Any());
            Assert.AreEqual(1, results.Count());
        }

        [Test]
        public async Task GetAll_ShouldThrowException_WhenNoClaimsExist()
        {
            // Remove existing claims
            var allClaims = _context.InsuranceClaims.ToList();
            _context.InsuranceClaims.RemoveRange(allClaims);
            await _context.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetAll());
            Assert.AreEqual("No claims found", ex.Message);
        }
    }
}
