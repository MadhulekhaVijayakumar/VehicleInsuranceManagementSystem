using InsuranceAPI.Controllers;
using InsuranceAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests.Controllers
{
    public class DocumentControllerTests
    {
        private Mock<IDocumentService> _mockDocumentService;
        private DocumentController _controller;

        [SetUp]
        public void Setup()
        {
            _mockDocumentService = new Mock<IDocumentService>();
            _controller = new DocumentController(_mockDocumentService.Object);

            // Fake Admin Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };
        }

        [Test]
        public async Task DownloadClaimDocument_ReturnsFile_WhenDocumentExists()
        {
            // Arrange
            var claimId = 123;
            var fileType = "pdf";
            var expectedBytes = new byte[] { 1, 2, 3 };
            var fileName = "claim-doc.pdf";

            _mockDocumentService
                .Setup(s => s.DownloadClaimDocumentAsync(claimId, fileType))
                .ReturnsAsync((expectedBytes, fileName));

            // Act
            var result = await _controller.DownloadClaimDocument(claimId, fileType);

            // Assert
            var fileResult = result as FileContentResult;
            Assert.IsNotNull(fileResult);
            Assert.AreEqual("application/pdf", fileResult.ContentType);
            Assert.AreEqual(expectedBytes, fileResult.FileContents);
            Assert.AreEqual(fileName, fileResult.FileDownloadName);
        }

        [Test]
        public async Task DownloadClaimDocument_ReturnsNotFound_WhenDocumentMissing()
        {
            // Arrange
            _mockDocumentService
                .Setup(s => s.DownloadClaimDocumentAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((ValueTuple<byte[], string>?)null);

            // Act
            var result = await _controller.DownloadClaimDocument(1, "pdf");

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task DownloadProposalDocument_ReturnsFile_WhenDocumentExists()
        {
            // Arrange
            var proposalId = 456;
            var fileType = "docx";
            var fileData = new byte[] { 4, 5, 6 };
            var fileName = "proposal.docx";

            _mockDocumentService
                .Setup(s => s.DownloadProposalDocumentAsync(proposalId, fileType))
                .ReturnsAsync((fileData, fileName));

            // Act
            var result = await _controller.DownloadProposalDocument(proposalId, fileType);

            // Assert
            var fileResult = result as FileContentResult;
            Assert.IsNotNull(fileResult);
            Assert.AreEqual("application/octet-stream", fileResult.ContentType);
            Assert.AreEqual(fileName, fileResult.FileDownloadName);
            Assert.AreEqual(fileData, fileResult.FileContents);
        }

        [Test]
        public async Task DownloadProposalDocument_ReturnsNotFound_WhenDocumentMissing()
        {
            // Arrange
            _mockDocumentService
                .Setup(s => s.DownloadProposalDocumentAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((ValueTuple<byte[], string>?)null);

            // Act
            var result = await _controller.DownloadProposalDocument(999, "txt");

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}
