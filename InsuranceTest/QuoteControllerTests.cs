using InsuranceAPI.Controllers;
using InsuranceAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests.Controllers
{
    public class QuoteControllerTests
    {
        private Mock<IQuoteService> _mockQuoteService;
        private QuoteController _controller;

        [SetUp]
        public void Setup()
        {
            _mockQuoteService = new Mock<IQuoteService>();
            _controller = new QuoteController(_mockQuoteService.Object);

            // Add mock user claims for role-based auth
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "Client"),
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
        public async Task DownloadQuote_ReturnsPdfFile_WhenSuccessful()
        {
            // Arrange
            int proposalId = 123;
            byte[] pdfBytes = new byte[] { 1, 2, 3 };

            _mockQuoteService
                .Setup(s => s.GenerateQuotePdfAsync(proposalId))
                .ReturnsAsync(pdfBytes);

            // Act
            var result = await _controller.DownloadQuote(proposalId);

            // Assert
            var fileResult = result as FileContentResult;
            Assert.IsNotNull(fileResult);
            Assert.AreEqual("application/pdf", fileResult.ContentType);
            Assert.AreEqual($"QUOTE-{proposalId:D5}.pdf", fileResult.FileDownloadName);
            Assert.AreEqual(pdfBytes, fileResult.FileContents);
        }

        [Test]
        public async Task DownloadQuote_ReturnsBadRequest_OnException()
        {
            // Arrange
            _mockQuoteService
                .Setup(s => s.GenerateQuotePdfAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.DownloadQuote(456);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual("Something went wrong", badRequest.Value);
        }
           
    }
}
