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
    public class AdminRepositoryTests
    {
        private InsuranceManagementContext _context;
        private AdminRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB for isolation
                .Options;

            _context = new InsuranceManagementContext(options);

            var admin = new Admin
            {
                Id = 1,
                Name = "AdminUser",
                Email = "admin@example.com",
               
            };

            _context.Admins.Add(admin);
            _context.SaveChanges();

            _repository = new AdminRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetById_ShouldReturnAdmin_WhenIdExists()
        {
            var result = await _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("AdminUser", result.Name);
            Assert.AreEqual("admin@example.com", result.Email);
        }

        [Test]
        public void GetById_ShouldThrowException_WhenAdminNotFound()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetById(99));
            Assert.AreEqual("Admin with  99 not found", ex.Message);
        }

        [Test]
        public async Task GetAll_ShouldReturnAllAdmins()
        {
            var result = await _repository.GetAll();

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetAll_ShouldThrowException_WhenNoAdminsFound()
        {
            _context.Admins.RemoveRange(_context.Admins);
            await _context.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetAll());
            Assert.AreEqual("No admins found", ex.Message);
        }
    }
}
