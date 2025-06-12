using InsuranceAPI.Context;
using InsuranceAPI.Models;
using InsuranceAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests.Repositories
{
    [TestFixture]
    public class ProposalRepositoryTests
    {
        private InsuranceManagementContext _context;
        private ProposalRepository _repository;

        [SetUp]
        
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new InsuranceManagementContext(options);

            // Add mock Client and Vehicle to satisfy FK constraints
            var client = new Client { Id = 1, Email = "test@example.com", Name = "Test Client" };
            var vehicle = new Vehicle { VehicleId = 1, VehicleNumber = "TN01AB1234", VehicleType = "Car", MakerName = "Honda", ModelName = "Civic", RegistrationDate = DateTime.Today.AddYears(-2) };

            _context.Clients.Add(client);
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();

            var proposal = new Proposal
            {
                ProposalId = 1,
                ClientId = client.Id,
                VehicleId = vehicle.VehicleId,
                InsuranceType = "Comprehensive",
                InsuranceValidUpto = DateTime.Today.AddYears(1),
                FitnessValidUpto = DateTime.Today.AddYears(1),
                Status = "submitted",
                CreatedAt = DateTime.Now,
                Documents = new List<Document>()
            };

            _context.Proposals.Add(proposal);
            _context.SaveChanges();

            _repository = new ProposalRepository(_context);
        }


        [Test]
        public async Task GetById_ValidId_ReturnsProposal()
        {
            var result = await _repository.GetById(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.ProposalId);
        }

        [Test]
        public void GetById_InvalidId_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetById(999));
            Assert.That(ex.Message, Is.EqualTo("Proposal with ID 999 not found"));
        }

        [Test]
        public async Task GetAll_WhenProposalsExist_ReturnsAllProposals()
        {
            var result = await _repository.GetAll();
            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty(result);
        }

        [Test]
        public void GetAll_WhenNoProposalsExist_ThrowsException()
        {
            _context.Proposals.RemoveRange(_context.Proposals);
            _context.SaveChanges();

            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetAll());
            Assert.That(ex.Message, Is.EqualTo("No proposals found"));
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
