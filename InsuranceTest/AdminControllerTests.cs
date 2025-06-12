using InsuranceAPI.Controllers;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests.Controllers
{
    [TestFixture]
    public class AdminControllerTests
    {
        private Mock<IAdminService> _mockAdminService;
        private Mock<IProposalService> _mockProposalService;
        private Mock<IClientService> _mockClientService;
        private AdminController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockAdminService = new Mock<IAdminService>();
            _mockProposalService = new Mock<IProposalService>();
            _mockClientService = new Mock<IClientService>();

            _controller = new AdminController(
                _mockAdminService.Object,
                _mockProposalService.Object,
                _mockClientService.Object);
        }

        [Test]
        public async Task CreateAdmin_ValidRequest_ReturnsCreatedResult()
        {
            // Arrange
            var request = new CreateAdminRequest
            {
                Name = "Luna",
                Email = "luna@example.com",
                Password = "StrongPassword"
            };

            var expectedResponse = new CreateAdminResponse
            {
                Id = 1,
                Message = "Admin created successfully"
            };

            _mockAdminService.Setup(s => s.CreateAdmin(request))
                             .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreateAdmin(request);

            // Assert
            var createdResult = result.Result as CreatedResult;
            Assert.IsNotNull(createdResult);
            var response = createdResult.Value as CreateAdminResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(expectedResponse.Id, response.Id);
            Assert.AreEqual("Admin created successfully", response.Message);
        }

        [Test]
        public async Task CreateAdmin_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new CreateAdminRequest
            {
                Name = "Luna",
                Email = "fail@example.com",
                Password = "FailPass"
            };

            _mockAdminService.Setup(s => s.CreateAdmin(request))
                             .ThrowsAsync(new Exception("Failed"));

            // Act
            var result = await _controller.CreateAdmin(request);

            // Assert
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("Failed", objectResult.Value);
        }

        [Test]
        public async Task SearchClients_KeywordMatch_ReturnsListOfClients()
        {
            // Arrange
            var keyword = "luna";
            var clients = new List<ClientProfileResponse>
            {
                new ClientProfileResponse
                {
                    Id = 1,
                    Name = "Luna",
                    Email = "luna@example.com",
                    PhoneNumber = "9999999999",
                    DateOfBirth = DateTime.Today.AddYears(-22),
                    Gender = "Female",
                    AadhaarNumber = "123412341234",
                    PANNumber = "ABCDE1234F",
                    Address = "Moon Street"
                }
            };

            _mockClientService.Setup(s => s.SearchClients(keyword))
                              .ReturnsAsync(clients);

            // Act
            var result = await _controller.SearchClients(keyword);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedClients = okResult.Value as IEnumerable<ClientProfileResponse>;
            Assert.IsNotNull(returnedClients);
            Assert.AreEqual(1, ((List<ClientProfileResponse>)returnedClients).Count);
        }

        [Test]
        public async Task SearchClients_NoMatches_ReturnsEmptyList()
        {
            // Arrange
            var keyword = "nonexistent";
            _mockClientService.Setup(s => s.SearchClients(keyword))
                              .ReturnsAsync(new List<ClientProfileResponse>());

            // Act
            var result = await _controller.SearchClients(keyword);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedClients = okResult.Value as IEnumerable<ClientProfileResponse>;
            Assert.IsNotNull(returnedClients);
            Assert.IsEmpty(returnedClients);
        }
    }
}
