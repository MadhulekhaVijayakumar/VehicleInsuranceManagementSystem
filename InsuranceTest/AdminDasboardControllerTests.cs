using Moq;
using NUnit.Framework;
using InsuranceAPI.Controllers;
using InsuranceAPI.Services;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using InsuranceAPI.Interfaces;

namespace InsuranceAPI.Tests
{
    public class AdminDashboardControllerTests
    {
        private Mock<IAdminDashboardService> _mockAdminDashboardService;
        private AdminDashboardController _adminDashboardController;

        [SetUp]
        public void SetUp()
        {
            _mockAdminDashboardService = new Mock<IAdminDashboardService>();
            _adminDashboardController = new AdminDashboardController(_mockAdminDashboardService.Object);
        }

        [Test]
        public async Task GetDashboardSummary_ShouldReturnOkResult()
        {
            // Arrange
            var summaryDto = new AdminDashboardSummaryDto
            {
                TotalClients = 2,
                PendingProposals = 3,
                ClaimsToReview = 1,
                TotalRevenue = 500.0m
            };

            _mockAdminDashboardService.Setup(s => s.GetDashboardSummaryAsync()).ReturnsAsync(summaryDto);

            // Act
            var result = await _adminDashboardController.GetDashboardSummary();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(summaryDto, okResult.Value);
        }
    }
}
