using InsuranceAPI.Context;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests.Services
{
    public class InsuranceClaimServiceTests
    {
        private Mock<IRepository<int, InsuranceClaim>> _mockClaimRepo;
        private Mock<IRepository<string, Insurance>> _mockInsuranceRepo;
        private Mock<IDocumentService> _mockDocumentService;
        private InsuranceClaimService _service;
        private InsuranceManagementContext _context;

        [SetUp]
        public void Setup()
        {
            // Set up in-memory database
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new InsuranceManagementContext(options);

            // Seed initial data
            SeedDatabase();

            // Initialize mocks
            _mockClaimRepo = new Mock<IRepository<int, InsuranceClaim>>();
            _mockInsuranceRepo = new Mock<IRepository<string, Insurance>>();
            _mockDocumentService = new Mock<IDocumentService>();

            var dummyHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var dummyActivityLogService = new Mock<IActivityLogService>();

            _service = new InsuranceClaimService(
                _mockClaimRepo.Object,
                _mockInsuranceRepo.Object,
                null,
                _mockDocumentService.Object,
                _context,
                dummyHttpContextAccessor.Object,
                dummyActivityLogService.Object
            );
        }

        private void SeedDatabase()
        {
            var client1 = new Client {
                Id=1,
                Name = "Alice Johnson",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Female",
                PhoneNumber = "9000011111",
                Email = "alice@example.com",
                AadhaarNumber = "123456789012",
                PANNumber = "ALICE1234J",
                Address = "Wonderland"
            };
            var client2 = new Client { Id = 2, Name = "Client Two",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Female",
                PhoneNumber = "9000011112",
                Email = "alicee@example.com",
                AadhaarNumber = "123456789015",
                PANNumber = "ALICE1234g",
                Address = "Wonderland"
            };

            var insurance1 = new Insurance
            {
                InsurancePolicyNumber = "POL123",
                ClientId = 1,
                Status = "active",
                PremiumAmount = 1000m,
                InsuranceSum = 50000m,
                InsuranceStartDate = DateTime.UtcNow
            };

            var insurance2 = new Insurance
            {
                InsurancePolicyNumber = "POL456",
                ClientId = 2,
                Status = "active",
                PremiumAmount = 1200m,
                InsuranceSum = 60000m,
                InsuranceStartDate = DateTime.UtcNow
            };

            var claim1 = new InsuranceClaim
            {
                ClaimId = 1,
                InsurancePolicyNumber = "POL123",
                IncidentDate = DateTime.UtcNow.AddDays(-1),
                ClaimAmount = 10000m,
                Status = "Pending",
                Insurance = insurance1
            };

            var claim2 = new InsuranceClaim
            {
                ClaimId = 2,
                InsurancePolicyNumber = "POL456",
                IncidentDate = DateTime.UtcNow.AddDays(-2),
                ClaimAmount = 20000m,
                Status = "Approved",
                Insurance = insurance2
            };

            _context.Clients.Add(client1);
            _context.Clients.Add(client2);
            _context.Insurances.Add(insurance1);
            _context.Insurances.Add(insurance2);
            _context.InsuranceClaims.Add(claim1);
            _context.InsuranceClaims.Add(claim2);
            _context.SaveChanges();
        }


        [Test]
        public async Task SubmitClaimWithDocumentsAsync_ValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var clientId = "123";
            var claimRequest = new CreateClaimRequest
            {
                InsurancePolicyNumber = "POL123",
                IncidentDate = DateTime.UtcNow.AddDays(-1),
                Description = "Accident on highway",
                ClaimAmount = 5000m,
                Documents = new ClaimDocumentUploadRequest()
            };

            var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, clientId) };
            var identity = new ClaimsIdentity(userClaims, "mock");
            var user = new ClaimsPrincipal(identity);

            var mockInsurance = new Insurance
            {
                InsurancePolicyNumber = "POL123",
                ClientId = int.Parse(clientId)
            };

            _mockInsuranceRepo.Setup(r => r.GetById("POL123")).ReturnsAsync(mockInsurance);

            _mockClaimRepo.Setup(r => r.Add(It.IsAny<InsuranceClaim>()))
                .ReturnsAsync((InsuranceClaim c) => {
                    c.ClaimId = 1; return c;
                });

            _mockDocumentService.Setup(d => d.SaveClaimDocumentsAsync(1, claimRequest.Documents))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.SubmitClaimWithDocumentsAsync(claimRequest, user);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ClaimId, Is.EqualTo(1));
            Assert.That(result.Status, Is.EqualTo("Pending"));
            Assert.That(result.Message, Is.EqualTo("Claim submitted successfully"));

            _mockInsuranceRepo.Verify(r => r.GetById("POL123"), Times.Once);
            _mockClaimRepo.Verify(r => r.Add(It.IsAny<InsuranceClaim>()), Times.Once);
            _mockDocumentService.Verify(d => d.SaveClaimDocumentsAsync(1, claimRequest.Documents), Times.Once);
        }
        [Test]
        public async Task GetClaimsByClientAsync_ValidClient_ReturnsClaims()
        {
            // Arrange
            var clientId = "1";  // Ensure this clientId corresponds to your seeded data
            var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, clientId) };
            var identity = new ClaimsIdentity(userClaims, "mock");
            var user = new ClaimsPrincipal(identity);

            // Mock InsuranceRepo to return the insurance policy for clientId "1"
            var mockInsurance = new Insurance
            {
                InsurancePolicyNumber = "POL123",
                ClientId = int.Parse(clientId)
            };

            _mockInsuranceRepo.Setup(r => r.GetById("POL123")).ReturnsAsync(mockInsurance);

            // Mock ClaimRepo to return claims related to this client
            var claims = new List<InsuranceClaim>
    {
        new InsuranceClaim
        {
            ClaimId = 1,
            InsurancePolicyNumber = "POL123",
            IncidentDate = DateTime.UtcNow.AddDays(-1),
            ClaimAmount = 10000m,
            Status = "Pending",
            Insurance = mockInsurance
        }
    };

            _mockClaimRepo.Setup(r => r.GetAll()).ReturnsAsync(claims);

            // Act
            var result = await _service.GetClaimsByClientAsync(user);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));  // This should match the claim for clientId 1
            Assert.That(result.First().ClaimId, Is.EqualTo(1));  // This should match the seeded claim
            Assert.That(result.First().InsurancePolicyNumber, Is.EqualTo("POL123"));  // This should match the insurance policy number
        }


        // Test GetAllPendingClaimsAsync method
        [Test]
        public async Task GetAllPendingClaimsAsync_ReturnsPendingClaims()
        {
            // Arrange
            // We're using the in-memory database with the seeded data

            // Act
            var result = await _service.GetAllPendingClaimsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().ClaimStatus, Is.EqualTo("Pending"));
            Assert.That(result.First().ClaimId, Is.EqualTo(1));
        }
       

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
