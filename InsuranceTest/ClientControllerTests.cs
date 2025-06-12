using Moq;
using NUnit.Framework;
using InsuranceAPI.Controllers;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using InsuranceAPI.Models;
using Azure.Core;

namespace InsuranceAPI.Tests.Controllers
{
    [TestFixture]
    public class ClientControllerTests
    {
        private Mock<IClientService> _mockClientService;
        private ClientController _controller;
        private Mock<HttpContext> _mockHttpContext;

        [SetUp]
        public void Setup()
        {
            _mockClientService = new Mock<IClientService>();
            _mockHttpContext = new Mock<HttpContext>();
            _controller = new ClientController(_mockClientService.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = _mockHttpContext.Object
                }
            };
        }

        [Test]
        public async Task CreateClient_ReturnsCreatedResult()
        {
            // Arrange
            var mockService = new Mock<IClientService>();
            mockService.Setup(x => x.CreateClient(It.IsAny<CreateClientRequest>()))
                       .ReturnsAsync(new CreateClientResponse { Id = 1 });

            var controller = new ClientController(mockService.Object);

            var request = new CreateClientRequest
            {
                Name = "Test",
                DateOfBirth = DateTime.UtcNow.AddYears(-30),
                Gender = "Male",
                PhoneNumber = "9876543210",
                Email = "test@client.com",
                Password = "password",
                AadhaarNumber = "123456789012",
                PANNumber = "ABCDE1234F",
                Address = "Test Address"
            };

            // Act
            var result = await controller.CreateClient(request);

            // Assert
            Assert.IsInstanceOf<CreatedResult>(result.Result);
            var createdResult = result.Result as CreatedResult;
            Assert.IsNotNull(createdResult);

            var response = createdResult.Value as CreateClientResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Id, Is.EqualTo(1));
        }


        [Test]
        public async Task GetProfile_ReturnsOkResult()
        {
            // Arrange
            var controller = new ClientController(_mockClientService.Object);
            var clientId = 1;

            _mockClientService.Setup(s => s.GetClientProfile(clientId))
                .ReturnsAsync(new ClientProfileResponse
                {
                    Id = clientId,
                    Name = "Test Client"
                });

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, clientId.ToString()),
        new Claim(ClaimTypes.Role, "Client")
    };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };

            // Act
            var result = await controller.GetProfile();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var response = okResult.Value as ClientProfileResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Id, Is.EqualTo(clientId));
        }

        [Test]
        public async Task UpdateProfile_ReturnsOkResult()
        {
            // Arrange
            var clientId = 1;

            var request = new UpdateClientRequest
            {
                NameUpdate = new NameUpdate { NewName = "Updated Name" }
                // Add other fields if needed
            };

            _mockClientService.Setup(s => s.UpdateClientProfile(clientId, request))
                .ReturnsAsync(new ClientProfileResponse
                {
                    Id = clientId,
                    Name = "Updated Name"
                    // Add other properties if needed
                });

            var controller = new ClientController(_mockClientService.Object);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, clientId.ToString()),
        new Claim(ClaimTypes.Role, "Client")
    };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };

            // Act
            var result = await controller.UpdateProfile(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var response = okResult.Value as ClientProfileResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Name, Is.EqualTo("Updated Name"));
        }


        [Test]
        public async Task ChangePassword_ShouldReturnOkResult_WhenValidPasswordChange()
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                OldPassword = "oldPassword123",
                NewPassword = "newPassword123"
            };

            _mockHttpContext.SetupGet(ctx => ctx.User)
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Email, "john@example.com") }, "mock")));

            _mockClientService.Setup(service => service.ChangeClientPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ChangePassword(request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Password changed successfully.", okResult.Value);
        }

        [Test]
        public async Task GetAllClients_ShouldReturnPaginatedResult_WhenValidRequest()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;

            var clients = new List<ClientProfileResponse>
            {
                new ClientProfileResponse { Id = 1, Name = "John Doe" },
                new ClientProfileResponse { Id = 2, Name = "Jane Doe" }
            };

            var paginatedResult = new PaginatedResult<ClientProfileResponse>
            {
                Data = clients,
                TotalRecords = 2,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            _mockClientService.Setup(service => service.GetAllClients(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetAllClients(pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(paginatedResult, okResult.Value);
        }
    }
}
