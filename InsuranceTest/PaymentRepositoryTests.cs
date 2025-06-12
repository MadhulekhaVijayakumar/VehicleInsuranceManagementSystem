using InsuranceAPI.Context;
using InsuranceAPI.Models;
using InsuranceAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests.Repositories
{
    [TestFixture]
    public class PaymentRepositoryTests
    {
        private InsuranceManagementContext _context;
        private PaymentRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new InsuranceManagementContext(options);

            // Seed a proposal
            var proposal = new Proposal
            {
                ProposalId = 1,
                ClientId = 101, // dummy client ID
                VehicleId = 201, // dummy vehicle ID
                InsuranceType = "Comprehensive",
                InsuranceValidUpto = DateTime.Today.AddYears(1),
                FitnessValidUpto = DateTime.Today.AddYears(1),
                Status = "submitted",
                CreatedAt = DateTime.Now
            };

            _context.Proposals.Add(proposal);

            // Seed a payment
            var payment = new Payment
            {
                PaymentId = 1001,
                ProposalId = 1,
                AmountPaid = 5000,
                PaymentDate = DateTime.Today,
                PaymentMode = "UPI",
                TransactionStatus = "Success",
                Proposal = proposal
            };
            _context.Payments.Add(payment);
            _context.SaveChanges();

            _repository = new PaymentRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetById_ShouldReturnPayment_WhenPaymentExists()
        {
            var result = await _repository.GetById(1001);

            Assert.IsNotNull(result);
            Assert.AreEqual(1001, result.PaymentId);
            Assert.AreEqual(5000, result.AmountPaid);
            Assert.AreEqual("UPI", result.PaymentMode);
        }

        [Test]
        public void GetById_ShouldThrowException_WhenPaymentNotFound()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetById(9999));
            Assert.AreEqual("Payment with ID 9999 not present", ex.Message);
        }

        [Test]
        public async Task GetAll_ShouldReturnAllPayments()
        {
            var result = await _repository.GetAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Success", result.First().TransactionStatus);
        }

        [Test]
        public async Task GetAll_ShouldThrowException_WhenNoPaymentsExist()
        {
            _context.Payments.RemoveRange(_context.Payments);
            await _context.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetAll());
            Assert.AreEqual("No payments found", ex.Message);
        }
    }
}
