using InsuranceAPI.Controllers;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceTest
{
    [TestFixture]
    public class InsuranceClaimControllerTest
    {
        private readonly InsuranceClaimController _controller;
        private readonly Mock<IInsuranceClaimService> _mockClaimService;
        private readonly Mock<IDocumentService> _mockDocumentService;
        private readonly Mock<ILogger<InsuranceClaimController>> _mockLogger;

        public InsuranceClaimControllerTest()
        {
            // Mock dependencies
            _mockClaimService = new Mock<IInsuranceClaimService>();
            _mockDocumentService = new Mock<IDocumentService>();
            _mockLogger = new Mock<ILogger<InsuranceClaimController>>();

            // Pass the dependencies in the correct order
            _controller = new InsuranceClaimController(
                _mockClaimService.Object,    // IInsuranceClaimService (first)
                _mockLogger.Object,          // ILogger<InsuranceClaimController> (second)
                _mockDocumentService.Object  // IDocumentService (third)
            );
        }

        // Add your test methods here...
        [Test]
        public async Task SubmitClaimWithDocumentsAsync_Returns_Success()
        {
            // Arrange
            var createClaimRequest = new CreateClaimRequest
            {
                InsurancePolicyNumber = "POL12345",
                IncidentDate = DateTime.UtcNow,
                Description = "Accident description",
                ClaimAmount = 1000,
                Documents = new ClaimDocumentUploadRequest() // Populate with test data
            };

            var expectedResponse = new CreateClaimResponse
            {
                ClaimId = 1,
                Status = "Pending",
                Message = "Claim submitted successfully"
            };

            // Setup the mock service to return the expected response
            _mockClaimService
                .Setup(service => service.SubmitClaimWithDocumentsAsync(It.IsAny<CreateClaimRequest>(), It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.SubmitClaimWithDocuments(createClaimRequest);

            // Assert
            Assert.IsInstanceOf<ActionResult<CreateClaimResponse>>(result);

            // Check if the result is an OkObjectResult
            var okResult = result.Result as OkObjectResult; // Correct the casting
            Assert.IsNotNull(okResult); // Ensure the result is of type OkObjectResult
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            // Ensure the response message matches the expected response
            var returnedResponse = okResult.Value as CreateClaimResponse;
            Assert.That(returnedResponse?.Message, Is.EqualTo(expectedResponse.Message));
        }




        [Test]
        public async Task GetAllPendingClaims_Returns_PendingClaims()
        {
            // Arrange
            var pendingClaims = new List<PendingClaimDto>
    {
        new PendingClaimDto { ClaimId = 1, InsurancePolicyNumber = "POL12345", ClaimAmount = 1000, ClaimStatus = "Pending" }
    };

            _mockClaimService
                .Setup(service => service.GetAllPendingClaimsAsync())
                .ReturnsAsync(pendingClaims);

            // Act
            var result = await _controller.GetAllPendingClaims();

            // Assert
            Assert.IsInstanceOf<ActionResult<List<PendingClaimDto>>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedClaims = okResult.Value as List<PendingClaimDto>;
            Assert.IsNotNull(returnedClaims);
            Assert.AreEqual(1, returnedClaims.Count());
            Assert.AreEqual("POL12345", returnedClaims.First().InsurancePolicyNumber);
            Assert.AreEqual("Pending", returnedClaims.First().ClaimStatus);
        }







    }

}
