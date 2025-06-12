using InsuranceAPI.Controllers;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests.Controllers
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private Mock<IAuthenticationService> _authServiceMock;
        private AuthenticationController _controller;

        [SetUp]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthenticationService>();
            _controller = new AuthenticationController(_authServiceMock.Object);
        }

        [Test]
        public async Task Login_WithValidAdminCredentials_ReturnsOkWithAdminMessage()
        {
            // Arrange
            var request = new UserLoginRequest { Username = "admin@example.com", Password = "adminpass" };
            var response = new LoginResponse { Id = 1, Name = "Admin One", Role = "Admin", Token = "jwt-token" };

            _authServiceMock.Setup(x => x.Login(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var json = System.Text.Json.JsonSerializer.Serialize(okResult!.Value);
            var parsed = System.Text.Json.JsonDocument.Parse(json).RootElement;

            Assert.AreEqual("Admin login successful", parsed.GetProperty("Message").GetString());

            var data = parsed.GetProperty("Data");
            Assert.AreEqual(response.Id, data.GetProperty("Id").GetInt32());
            Assert.AreEqual(response.Name, data.GetProperty("Name").GetString());
            Assert.AreEqual(response.Role, data.GetProperty("Role").GetString());
            Assert.AreEqual(response.Token, data.GetProperty("Token").GetString());
        }

        [Test]
        public async Task Login_WithValidClientCredentials_ReturnsOkWithClientMessage()
        {
            // Arrange
            var request = new UserLoginRequest
            {
                Username = "client@example.com",
                Password = "clientpass"
            };

            var response = new LoginResponse
            {
                Id = 2,
                Name = "Client One",
                Role = "Client",
                Token = "jwt-token-client"
            };

            _authServiceMock.Setup(x => x.Login(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var json = System.Text.Json.JsonSerializer.Serialize(okResult!.Value);
            var parsed = System.Text.Json.JsonDocument.Parse(json).RootElement;

            Assert.AreEqual("Client login successful", parsed.GetProperty("Message").GetString());

            var data = parsed.GetProperty("Data");
            Assert.AreEqual(response.Id, data.GetProperty("Id").GetInt32());
            Assert.AreEqual(response.Name, data.GetProperty("Name").GetString());
            Assert.AreEqual(response.Role, data.GetProperty("Role").GetString());
            Assert.AreEqual(response.Token, data.GetProperty("Token").GetString());
        }


        [Test]
        public async Task Login_WithInvalidRole_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserLoginRequest { Username = "unknown@example.com", Password = "userpass" };
            var response = new LoginResponse { Id = 3, Name = "Unknown", Role = "Other", Token = "jwt-token" };

            _authServiceMock.Setup(x => x.Login(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badResult);
            Assert.AreEqual("Invalid role.", badResult.Value);
        }

        [Test]
        public async Task Login_WithUnauthorizedCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var request = new UserLoginRequest { Username = "unauth@example.com", Password = "wrongpass" };

            _authServiceMock.Setup(x => x.Login(request))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result.Result);
            var unauthResult = result.Result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthResult);
            Assert.AreEqual("Invalid credentials", unauthResult.Value);
        }

        [Test]
        public async Task Login_WithUnexpectedException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new UserLoginRequest { Username = "error@example.com", Password = "pass" };

            _authServiceMock.Setup(x => x.Login(request))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var errorResult = result.Result as ObjectResult;
            Assert.IsNotNull(errorResult);
            Assert.AreEqual(500, errorResult.StatusCode);
            Assert.AreEqual("Unexpected error", errorResult.Value);
        }
    }
}
