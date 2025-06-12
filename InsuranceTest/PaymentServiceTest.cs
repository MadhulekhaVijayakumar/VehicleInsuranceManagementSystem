using InsuranceAPI.Context;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace InsuranceTest
{
    public class PaymentServiceTests
    {
        private InsuranceManagementContext _context;
        private PaymentService _paymentService;
        private ILogger<PaymentService> _logger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            _context = new InsuranceManagementContext(options);
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<PaymentService>();

            _paymentService = new PaymentService(_context, _logger);
        }

        [Test]
        public async Task ProcessPaymentAsync_ShouldCreatePaymentAndUpdateProposal()
        {
            // Arrange
            var proposal = new Proposal
            {
                ProposalId = 1,
                ClientId = 101,
                VehicleId = 201,
                InsuranceType = "Comprehensive",
                InsuranceValidUpto = DateTime.Now.AddYears(1),
                FitnessValidUpto = DateTime.Now.AddYears(1),
                Status = "pending"
            };


            _context.Proposals.Add(proposal);
            await _context.SaveChangesAsync();

            var request = new CreatePaymentRequest
            {
                ProposalId = 1,
                AmountPaid = 5000.00m,
                PaymentMode = "UPI"
            };

            // Act
            var result = await _paymentService.ProcessPaymentAsync(request);

            // Assert
            var paymentInDb = await _context.Payments.FirstOrDefaultAsync(p => p.ProposalId == 1);
            var updatedProposal = await _context.Proposals.FindAsync(1);

            Assert.IsNotNull(paymentInDb);
            Assert.AreEqual("success", paymentInDb.TransactionStatus);
            Assert.AreEqual("payment successful", updatedProposal.Status);
        }

        [Test]
        public async Task ProcessPaymentAsync_ShouldReturnMessage_WhenPaymentAlreadyExists()
        {
            // Arrange
            var proposal = new Proposal
            {
                ProposalId = 2,
                ClientId = 102,
                VehicleId = 202,
                InsuranceType = "Comprehensive",
                InsuranceValidUpto = DateTime.Now.AddYears(1),
                FitnessValidUpto = DateTime.Now.AddYears(1),
                Status = "pending"
            };

            var existingPayment = new Payment
            {
                ProposalId = 2,
                AmountPaid = 3000.00m,
                PaymentDate = DateTime.Now.AddDays(-1),
                PaymentMode = "Card",
                TransactionStatus = "success"
            };

            _context.Proposals.Add(proposal);
            _context.Payments.Add(existingPayment);
            await _context.SaveChangesAsync();

            var request = new CreatePaymentRequest
            {
                ProposalId = 2,
                AmountPaid = 3000.00m,
                PaymentMode = "Card"
            };

            // Act
            var result = await _paymentService.ProcessPaymentAsync(request);

            // Use reflection to get "Message" property
            var messageProperty = result.GetType().GetProperty("Message");
            var message = messageProperty?.GetValue(result)?.ToString();

            // Assert
            Assert.That(message, Is.EqualTo("Payment has already been made for this proposal."));
        }


        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
