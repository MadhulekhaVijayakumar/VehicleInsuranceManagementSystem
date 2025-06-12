using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Models;
using InsuranceAPI.Services;
using InsuranceAPI.Repositories;

namespace InsuranceTest
{
    [TestFixture]
    public class ProposalServiceTests
    {
        private Mock<IRepository<int, Proposal>> _mockProposalRepo;
        private Mock<IRepository<int, Vehicle>> _mockVehicleRepo;
        private Mock<IRepository<int, InsuranceDetails>> _mockInsuranceDetailsRepo;
        private Mock<IRepository<int, Document>> _mockDocumentRepo;
        private Mock<IPremiumCalculatorService> _mockPremiumCalculator;
        private Mock<IDocumentService> _mockDocumentService;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IActivityLogService> _mockActivityLogService;
        private Mock<ILogger<ProposalService>> _mockLogger;
        private ProposalService _service;

        [SetUp]
        public void Setup()
        {
            _mockProposalRepo = new Mock<IRepository<int, Proposal>>();
            _mockVehicleRepo = new Mock<IRepository<int, Vehicle>>();
            _mockInsuranceDetailsRepo = new Mock<IRepository<int, InsuranceDetails>>();
            _mockDocumentRepo = new Mock<IRepository<int, Document>>();
            _mockPremiumCalculator = new Mock<IPremiumCalculatorService>();
            _mockDocumentService = new Mock<IDocumentService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockActivityLogService = new Mock<IActivityLogService>();
            _mockLogger = new Mock<ILogger<ProposalService>>();

            _service = new ProposalService(
                _mockProposalRepo.Object,
                _mockVehicleRepo.Object,
                _mockInsuranceDetailsRepo.Object,
                _mockDocumentRepo.Object,
                _mockPremiumCalculator.Object,
                _mockDocumentService.Object,
                _mockLogger.Object,
                _mockHttpContextAccessor.Object,
                _mockActivityLogService.Object
            );
        }

        [Test]
        public async Task SubmitProposalWithDetails_ShouldReturnResponse_WhenSuccessful()
        {
            // Arrange
            var clientId = 101;
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, clientId.ToString())
            }));
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

            var vehicle = new Vehicle { VehicleId = 1, ClientId = clientId };
            var proposal = new Proposal { ProposalId = 1, ClientId = clientId, VehicleId = vehicle.VehicleId, Status = "submitted" };

            var insuranceDetails = new InsuranceDetails
            {
                ProposalId = proposal.ProposalId,
                VehicleId = vehicle.VehicleId,
                InsuranceStartDate = DateTime.Now,
                InsuranceSum = 100000,
                DamageInsurance = "Full",
                LiabilityOption = "Third Party",
                Plan = "Gold"
            };

            _mockVehicleRepo.Setup(r => r.Add(It.IsAny<Vehicle>())).ReturnsAsync(vehicle);
            _mockProposalRepo.Setup(r => r.Add(It.IsAny<Proposal>())).ReturnsAsync(proposal);
            _mockInsuranceDetailsRepo.Setup(r => r.Add(It.IsAny<InsuranceDetails>()))
     .ReturnsAsync(new InsuranceDetails
     {
         ProposalId = 1,
         VehicleId = 1,
         InsuranceStartDate = DateTime.Now,
         InsuranceSum = 100000,
         DamageInsurance = "Comprehensive",
         LiabilityOption = "Full",
         Plan = "Premium",
         CalculatedPremium = 1234.56m
     });

            _mockPremiumCalculator.Setup(p => p.CalculatePremium(It.IsAny<InsuranceDetails>(), It.IsAny<Vehicle>())).Returns(5000);
            _mockDocumentService.Setup(d => d.SaveDocumentsAsync(It.IsAny<ProposalDocumentUploadRequest>())).Returns(Task.CompletedTask);

            var mockFile = new Mock<IFormFile>();
            var content = "Dummy file content";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.FileName).Returns("license.pdf");
            mockFile.Setup(f => f.Length).Returns(stream.Length);

            var request = new CreateProposalRequest
            {
                Vehicle = new CreateVehicleRequest
                {
                    VehicleNumber = "MH01AB1234",
                    VehicleType = "Car",
                    ChassisNumber = "CH123456",
                    EngineNumber = "EN123456",
                    MakerName = "Maruti",
                    ModelName = "Swift",
                    VehicleColor = "Red",
                    FuelType = "Petrol",
                    RegistrationDate = DateTime.Now.AddYears(-1),
                    SeatCapacity = 5
                },
                Proposal = new CreateProposalData
                {
                    InsuranceType = "Comprehensive",
                    InsuranceValidUpto = DateTime.Now.AddYears(1),
                    FitnessValidUpto = DateTime.Now.AddYears(1)
                },
                InsuranceDetails = new CreateInsuranceDetailRequest
                {
                    InsuranceSum = 100000,
                    InsuranceStartDate = DateTime.Now,
                    Plan = "Gold",
                    DamageInsurance = "Full",
                    LiabilityOption = "Third Party"
                },
                Documents = new ProposalDocumentUploadRequest
                {
                    License = mockFile.Object,
                    RCBook = mockFile.Object,
                    PollutionCertificate = mockFile.Object
                }
            };

            // Act
            var result = await _service.SubmitProposalWithDetails(request);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.ProposalId);
            Assert.AreEqual("submitted", result.Status);
            Assert.AreEqual(5000, result.CalculatedPremium);
        }

        [Test]
        public async Task GetSubmittedProposals_ShouldReturnSubmittedProposals()
        {
            // Arrange
            var proposals = new List<Proposal>
    {
        new Proposal
        {
            ProposalId = 1,
            ClientId = 101,
            Status = "submitted",
            InsuranceType = "Comprehensive",
            Client = new Client { Name = "Alice" }
        },
        new Proposal
        {
            ProposalId = 2,
            ClientId = 102,
            Status = "submitted",
            InsuranceType = "Third Party",
            Client = new Client { Name = "Bob" }
        },
        new Proposal
        {
            ProposalId = 3,
            ClientId = 103,
            Status = "approved", // Should be filtered out
            InsuranceType = "Comprehensive",
            Client = new Client { Name = "Charlie" }
        }
    };

            _mockProposalRepo.Setup(r => r.GetAll()).ReturnsAsync(proposals);

            // Act
            var result = (await _service.GetSubmittedProposals()).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(p => p.Status == "submitted"));
            Assert.AreEqual("Alice", result[0].ClientName);
            Assert.AreEqual("Bob", result[1].ClientName);
            Assert.AreEqual("Comprehensive", result[0].InsuranceType);
            Assert.AreEqual("Third Party", result[1].InsuranceType);
        }


        [Test]
        public async Task GetApprovedProposals_ShouldReturnApprovedProposals()
        {
            // Arrange
            var proposals = new List<Proposal>
    {
        new Proposal
        {
            ProposalId = 1,
            ClientId = 201,
            Status = "approved",
            InsuranceType = "Comprehensive",
            Client = new Client { Name = "David" }
        },
        new Proposal
        {
            ProposalId = 2,
            ClientId = 202,
            Status = "approved",
            InsuranceType = "Third Party",
            Client = new Client { Name = "Eva" }
        },
        new Proposal
        {
            ProposalId = 3,
            ClientId = 203,
            Status = "submitted", // should not be included
            InsuranceType = "Comprehensive",
            Client = new Client { Name = "Frank" }
        }
    };

            _mockProposalRepo.Setup(r => r.GetAll()).ReturnsAsync(proposals);

            // Act
            var result = (await _service.GetApprovedProposals()).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(p => p.Status == "approved"));
            Assert.AreEqual("David", result[0].ClientName);
            Assert.AreEqual("Eva", result[1].ClientName);
            Assert.AreEqual("Comprehensive", result[0].InsuranceType);
            Assert.AreEqual("Third Party", result[1].InsuranceType);
        }

        [Test]
        public async Task VerifyProposal_ShouldUpdateStatusToApproved_AndLogActivity()
        {
            // Arrange
            var proposal = new Proposal { ProposalId = 1, Status = "submitted" };

            // Setup the repository and other mocks
            _mockProposalRepo.Setup(r => r.GetById(1)).ReturnsAsync(proposal);  // Mock GetById to return the proposal
            _mockProposalRepo.Setup(r => r.Update(1, It.IsAny<Proposal>())).ReturnsAsync(proposal);  // Mock Update to return the updated proposal
            _mockHttpContextAccessor.Setup(h => h.HttpContext.User.Identity.Name).Returns("AdminUser");  // Mock Admin username
            _mockActivityLogService.Setup(a => a.LogActivityAsync("AdminUser", It.IsAny<string>())).Returns(Task.CompletedTask);  // Mock LogActivityAsync to return a completed task

            // Act
            var result = await _service.VerifyProposal(1, true);  // Call VerifyProposal with proposalId and approve set to true

            // Assert
            Assert.IsTrue(result);  // The result should be true, indicating the proposal was verified successfully.
            Assert.AreEqual("approved", proposal.Status);  // Check that the proposal status is updated to "approved"

            // Verify that Update was called once with the updated proposal
            _mockProposalRepo.Verify(r => r.Update(1, proposal), Times.Once);

            // Verify that LogActivityAsync was called once with the correct message
            _mockActivityLogService.Verify(a => a.LogActivityAsync("AdminUser", "Verified proposal #1: approved"), Times.Once);
        }


        [Test]
        public async Task GetProposalDetailsByProposalId_ShouldReturnFullProposalDetails()
        {
            // Arrange
            var proposal = new Proposal
            {
                ProposalId = 1,
                ClientId = 10,
                VehicleId = 20,
                InsuranceType = "Car",
                InsuranceValidUpto = DateTime.Today.AddYears(1),
                FitnessValidUpto = DateTime.Today.AddYears(1),
                InsuranceDetails = new InsuranceDetails
                {
                    InsuranceStartDate = DateTime.Today,
                    InsuranceSum = 500000,
                    DamageInsurance = "Full",
                    LiabilityOption = "ThirdParty",
                    Plan = "Premium"
                }
            };

            var vehicle = new Vehicle
            {
                VehicleId = 20,
                VehicleType = "Car",
                VehicleNumber = "MH01AB1234",
                ChassisNumber = "CH123456",
                EngineNumber = "EN123456",
                MakerName = "Honda",
                ModelName = "City",
                VehicleColor = "Red",
                FuelType = "Petrol",
                RegistrationDate = new DateTime(2022, 1, 1),
                SeatCapacity = 5
            };

            // Replace ProposalDocument with Document.
            var documents = new List<Document>
{
    new Document { ProposalId = 1, FileType = "License", FileName = "license.pdf" },
    new Document { ProposalId = 1, FileType = "RC Book", FileName = "rcbook.pdf" },
    new Document { ProposalId = 1, FileType = "Pollution Certificate", FileName = "pollution.pdf" },
    new Document { ProposalId = 2, FileType = "License", FileName = "other.pdf" } // unrelated
};


            _mockProposalRepo.Setup(r => r.GetById(1)).ReturnsAsync(proposal);
            _mockVehicleRepo.Setup(r => r.GetById(20)).ReturnsAsync(vehicle);
            // Assuming Document is the actual type in your repository.
            _mockDocumentRepo.Setup(r => r.GetAll()).ReturnsAsync(documents.AsEnumerable());


            // Act
            var result = await _service.GetProposalDetailsByProposalIdAsync(1);

            // Assert
            Assert.NotNull(result);

            // ClientId
            Assert.AreEqual(10, result.ClientId);

            // Proposal
            Assert.AreEqual("Car", result.Proposal.InsuranceType);
            Assert.AreEqual(proposal.InsuranceValidUpto, result.Proposal.InsuranceValidUpto);
            Assert.AreEqual(proposal.FitnessValidUpto, result.Proposal.FitnessValidUpto);

            // Vehicle
            Assert.AreEqual("Car", result.Vehicle.VehicleType);
            Assert.AreEqual("MH01AB1234", result.Vehicle.VehicleNumber);
            Assert.AreEqual("CH123456", result.Vehicle.ChassisNumber);
            Assert.AreEqual("EN123456", result.Vehicle.EngineNumber);
            Assert.AreEqual("Honda", result.Vehicle.MakerName);
            Assert.AreEqual("City", result.Vehicle.ModelName);
            Assert.AreEqual("Red", result.Vehicle.VehicleColor);
            Assert.AreEqual("Petrol", result.Vehicle.FuelType);
            Assert.AreEqual(new DateTime(2022, 1, 1), result.Vehicle.RegistrationDate);
            Assert.AreEqual(5, result.Vehicle.SeatCapacity);

            // InsuranceDetails
            Assert.AreEqual(DateTime.Today, result.InsuranceDetails.InsuranceStartDate);
            Assert.AreEqual(500000, result.InsuranceDetails.InsuranceSum);
            Assert.AreEqual("Full", result.InsuranceDetails.DamageInsurance);
            Assert.AreEqual("ThirdParty", result.InsuranceDetails.LiabilityOption);
            Assert.AreEqual("Premium", result.InsuranceDetails.Plan);

            // Documents
            Assert.AreEqual("license.pdf", result.Documents.LicenseFileName);
            Assert.AreEqual("rcbook.pdf", result.Documents.RCBookFileName);
            Assert.AreEqual("pollution.pdf", result.Documents.PollutionCertificateFileName);
        }


    }
}
