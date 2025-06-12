using InsuranceAPI.Context;
using InsuranceAPI.Models;
using InsuranceAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace InsuranceTest
{
    public class ClientRepositoryTests
    {
        private InsuranceManagementContext _context;
        private ClientRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            _context = new InsuranceManagementContext(options);
            _repository = new ClientRepository(_context);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetById_ShouldReturnClient_WhenClientExists()
        {
            // Arrange
            var client = new Client
            {
                Id = 1,
                Name = "Karthi",
                DateOfBirth = new DateTime(2000, 1, 1),
                Gender = "Male",
                PhoneNumber = "9876543210",
                Email = "karthi@mail.com",
                AadhaarNumber = "123456789012",
                PANNumber = "ABCDE1234F",
                Address = "Chennimalai"
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Karthi", result.Name);
        }

        [Test]
        public void GetById_ShouldThrowException_WhenClientNotFound()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetById(99));
            Assert.That(ex.Message, Is.EqualTo("Client with ID 99 not present"));
        }
        [Test]
        public async Task GetAll_ShouldReturnClients_WhenClientsExist()
        {
            var user = new User
            {
                Username = "sarah@mail.com",
                Password = Encoding.UTF8.GetBytes("password123"),
                HashKey = Encoding.UTF8.GetBytes("secretkey123"),
                Role = "Client"
            };

            var client = new Client
            {
                Id = 2,
                Name = "Sarah",
                DateOfBirth = new DateTime(2001, 2, 2),
                Gender = "Female",
                PhoneNumber = "9876543211",
                Email = "sarah@mail.com",
                AadhaarNumber = "111122223333",
                PANNumber = "XYZAB1234P",
                Address = "Erode",
                User = user
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var clients = await _repository.GetAll();

            Assert.IsNotNull(clients);
            Assert.That(clients, Has.Exactly(1).Items);
        }


        [Test]
        public void GetAll_ShouldThrowException_WhenNoClientsFound()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetAll());
            Assert.That(ex.Message, Is.EqualTo("No clients found"));
        }
    }
}