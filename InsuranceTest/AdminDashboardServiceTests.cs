using NUnit.Framework;
using InsuranceAPI.Services;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using InsuranceAPI.Models;

namespace InsuranceAPI.Tests
{
    public class AdminDashboardServiceTests
    {
        private InsuranceManagementContext _context;
        private AdminDashboardService _adminDashboardService;

        [SetUp]
        public void SetUp()
        {
            // Use an in-memory database for testing
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestDb") // Unique in-memory database name
                .Options;

            _context = new InsuranceManagementContext(options);

            // Seed the database with mock data
            SeedDatabase();

            _adminDashboardService = new AdminDashboardService(_context);
        }

        private void SeedDatabase()
        {
            // Add test data to the context
            _context.Clients.Add(new Client());
            _context.Clients.Add(new Client());

            // Add Proposals with all required fields
            _context.Proposals.Add(new Proposal
            {
                Status = "submitted",
                InsuranceValidUpto = DateTime.Now.AddYears(1),  // Add a valid value for the required field
                FitnessValidUpto = DateTime.Now.AddMonths(6),
                InsuranceType = "Comprehensive",
                CreatedAt = DateTime.Now,
                ClientId = 1, // Assuming ClientId is valid
                VehicleId = 1  // Assuming VehicleId is valid
            });

            _context.Proposals.Add(new Proposal
            {
                Status = "submitted",
                InsuranceValidUpto = DateTime.Now.AddYears(1),
                FitnessValidUpto = DateTime.Now.AddMonths(6),
                InsuranceType = "Third-Party",
                CreatedAt = DateTime.Now,
                ClientId = 2,
                VehicleId = 2
            });

            _context.Proposals.Add(new Proposal
            {
                Status = "active",
                InsuranceValidUpto = DateTime.Now.AddYears(1),
                FitnessValidUpto = DateTime.Now.AddMonths(6),
                InsuranceType = "Comprehensive",
                CreatedAt = DateTime.Now,
                ClientId = 1,
                VehicleId = 2
            });

            // Add InsuranceClaims
            _context.InsuranceClaims.Add(new InsuranceClaim
            {
                Status = "pending",
                CreatedAt = DateTime.Now,
                ClaimAmount = 500.0m,
                IncidentDate = DateTime.Now.AddMonths(-1),
                InsurancePolicyNumber = "POLICY123"
            });

            // Add Payments
            _context.Payments.Add(new Payment { AmountPaid = 100.0m });
            _context.Payments.Add(new Payment { AmountPaid = 150.0m });

            _context.SaveChanges();
        }


        [Test]
        public async Task GetDashboardSummaryAsync_ShouldReturnCorrectSummary()
        {
            // Act
            var result = await _adminDashboardService.GetDashboardSummaryAsync();

            // Assert
            Assert.AreEqual(2, result.TotalClients);
            Assert.AreEqual(2, result.PendingProposals);
            Assert.AreEqual(1, result.ClaimsToReview);
            Assert.AreEqual(250.0m, result.TotalRevenue);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
