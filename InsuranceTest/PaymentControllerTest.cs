using InsuranceAPI.Controllers;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests.Controllers
{
    public class PaymentControllerTests
    {
        private Mock<IPaymentService> _paymentServiceMock;
        private Mock<ILogger<PaymentController>> _loggerMock;
        private PaymentController _controller;

        [SetUp]
        public void Setup()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _loggerMock = new Mock<ILogger<PaymentController>>();
            _controller = new PaymentController(_paymentServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task MakePayment_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new CreatePaymentRequest
            {
                ProposalId = 1,
                AmountPaid = 1000,
                PaymentMode = "UPI"
            };

            var response = new PaymentResponse
            {
                PaymentId = 1001,
                ProposalId = 1,
                AmountPaid = 1000,
                PaymentDate = DateTime.Now,
                PaymentMode = "UPI",
                TransactionStatus = "success"
            };

            _paymentServiceMock
                .Setup(service => service.ProcessPaymentAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.MakePayment(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<PaymentResponse>(okResult.Value);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task MakePayment_PaymentAlreadyExists_ReturnsConflict()
        {
            // Arrange
            var request = new CreatePaymentRequest { ProposalId = 1, AmountPaid = 1000, PaymentMode = "Card" };

            var conflictResult = new { Message = "Payment has already been made for this proposal." };

            _paymentServiceMock
                .Setup(s => s.ProcessPaymentAsync(request))
                .ReturnsAsync(conflictResult);

            // Act
            var result = await _controller.MakePayment(request);

            // Assert
            Assert.IsInstanceOf<ConflictObjectResult>(result);
            var conflict = result as ConflictObjectResult;
            Assert.AreEqual(409, conflict.StatusCode);
        }

        [Test]
        public async Task MakePayment_InvalidRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.MakePayment(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = result as BadRequestObjectResult;
            Assert.AreEqual("Invalid payment request.", badRequest.Value);
        }

        [Test]
        public async Task MakePayment_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new CreatePaymentRequest { ProposalId = 5, AmountPaid = 500, PaymentMode = "NetBanking" };

            _paymentServiceMock
                .Setup(s => s.ProcessPaymentAsync(request))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.MakePayment(request);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objResult = result as ObjectResult;
            Assert.AreEqual(500, objResult.StatusCode);
            Assert.That(objResult.Value.ToString(), Does.Contain("Payment failed"));
        }
    }
}
