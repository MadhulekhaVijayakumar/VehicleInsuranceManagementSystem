using InsuranceAPI.Context;
using InsuranceAPI.Models;
using InsuranceAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests.Repositories
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private InsuranceManagementContext _context;
        private UserRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new InsuranceManagementContext(options);
            _repository = new UserRepository(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Add_ShouldInsertUser()
        {
            var user = new User
            {
                Username = "testuser",
                Role = "Client",
                Password = System.Text.Encoding.UTF8.GetBytes("password123"),
                HashKey = System.Text.Encoding.UTF8.GetBytes("key123")
            };

            var result = await _repository.Add(user);

            Assert.IsNotNull(result);
            Assert.AreEqual("testuser", result.Username);
        }

        [Test]
        public async Task GetById_ShouldReturnUser_WhenUserExists()
        {
            var user = new User
            {
                Username = "john_doe",
                Role = "Admin",
                Password = System.Text.Encoding.UTF8.GetBytes("secret"),
                HashKey = System.Text.Encoding.UTF8.GetBytes("hashkey")
            };

            await _repository.Add(user);

            var result = await _repository.GetById("john_doe");

            Assert.IsNotNull(result);
            Assert.AreEqual("john_doe", result.Username);
        }

        [Test]
        public void GetById_ShouldThrowException_WhenUserDoesNotExist()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _repository.GetById("non_existing");
            });

            Assert.AreEqual("User not found", ex.Message);
        }

        [Test]
        public async Task GetAll_ShouldReturnAllUsers()
        {
            var user1 = new User
            {
                Username = "user1",
                Role = "Client",
                Password = System.Text.Encoding.UTF8.GetBytes("pwd1"),
                HashKey = System.Text.Encoding.UTF8.GetBytes("hk1")
            };

            var user2 = new User
            {
                Username = "user2",
                Role = "Agent",
                Password = System.Text.Encoding.UTF8.GetBytes("pwd2"),
                HashKey = System.Text.Encoding.UTF8.GetBytes("hk2")
            };

            await _repository.Add(user1);
            await _repository.Add(user2);

            var users = await _repository.GetAll();
            var userList = users.ToList();

            Assert.IsNotNull(userList);
            Assert.AreEqual(2, userList.Count);
        }

    }
}