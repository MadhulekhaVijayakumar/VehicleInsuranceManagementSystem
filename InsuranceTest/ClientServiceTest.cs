using AutoMapper;
using InsuranceAPI.Context;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Repositories;
using InsuranceAPI.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Security.Cryptography;
using System.Text;

namespace InsuranceTest

{
    [TestFixture]
    public class ClientServiceTests
    {
        private IMapper _mapper;
        private ClientService _clientService;
        private IRepository<string, User> _userRepository;
        private IRepository<int, Client> _clientRepository;
        private InsuranceManagementContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new InsuranceManagementContext(options);
            _userRepository = new UserRepository(_context);
            _clientRepository = new ClientRepository(_context);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Client, ClientProfileResponse>();
            });

            _mapper = config.CreateMapper();
            _clientService = new ClientService(_userRepository, _clientRepository, _mapper);
        }

        [Test]
        public async Task CreateClient_ShouldReturnClientResponse()
        {
            var request = new CreateClientRequest
            {
                Name = "Alice Johnson",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Female",
                PhoneNumber = "9000011111",
                Email = "alice@example.com",
                AadhaarNumber = "123456789012",
                PANNumber = "ALICE1234J",
                Address = "Wonderland",
                Password = "SecurePass1"
            };

            var result = await _clientService.CreateClient(request);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Id, Is.GreaterThan(0));
            });
        }

        [Test]
        public async Task GetClientProfile_ShouldReturnProfile()
        {
            var request = new CreateClientRequest
            {
                Name = "Bob Smith",
                DateOfBirth = new DateTime(1985, 6, 15),
                Gender = "Male",
                PhoneNumber = "9887766554",
                Email = "bob@example.com",
                AadhaarNumber = "111122223333",
                PANNumber = "BOBSM1234S",
                Address = "Springfield",
                Password = "TestPass123"
            };

            var created = await _clientService.CreateClient(request);
            var profile = await _clientService.GetClientProfile(created.Id);

            Assert.Multiple(() =>
            {
                Assert.That(profile.Name, Is.EqualTo("Bob Smith"));
                Assert.That(profile.Email, Is.EqualTo("bob@example.com"));
            });
        }

        [Test]
        public async Task UpdateClientProfile_ShouldUpdateSuccessfully()
        {
            var request = new CreateClientRequest
            {
                Name = "Charlie Brown",
                DateOfBirth = new DateTime(1995, 9, 9),
                Gender = "Male",
                PhoneNumber = "9111222233",
                Email = "charlie@example.com",
                AadhaarNumber = "444455556666",
                PANNumber = "CHARL1234B",
                Address = "Peanuts Town",
                Password = "OldPassword1"
            };

            var created = await _clientService.CreateClient(request);

            var updateRequest = new UpdateClientRequest
            {
                NameUpdate = new NameUpdate { NewName = "Charles Brown" },
                PhoneUpdate = new PhoneUpdate { NewPhoneNumber = "9111000000" }
            };

            var updated = await _clientService.UpdateClientProfile(created.Id, updateRequest);

            Assert.Multiple(() =>
            {
                Assert.That(updated.Name, Is.EqualTo("Charles Brown"));
                Assert.That(updated.PhoneNumber, Is.EqualTo("9111000000"));
            });
        }

        [Test]
        public async Task ChangeClientPassword_ShouldSucceedWithCorrectOldPassword()
        {
            var request = new CreateClientRequest
            {
                Name = "Diana Prince",
                DateOfBirth = new DateTime(1988, 4, 4),
                Gender = "Female",
                PhoneNumber = "9999888877",
                Email = "diana@example.com",
                AadhaarNumber = "999988887777",
                PANNumber = "DIANA1234P",
                Address = "Themyscira",
                Password = "Original123"
            };

            await _clientService.CreateClient(request);

            var result = await _clientService.ChangeClientPassword("diana@example.com", "Original123", "NewSecure123");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task GetAllClients_ShouldReturnPaginatedClients()
        {
            for (int i = 0; i < 5; i++)
            {
                await _clientService.CreateClient(new CreateClientRequest
                {
                    Name = $"Client {i}",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Gender = "Other",
                    PhoneNumber = $"900000000{i}",
                    Email = $"client{i}@example.com",
                    AadhaarNumber = $"11112222333{i}",
                    PANNumber = $"PANCL{i}1234",
                    Address = $"Address {i}",
                    Password = "Password123"
                });
            }

            var pagedResult = await _clientService.GetAllClients(1, 3);

            Assert.Multiple(() =>
            {
                Assert.That(pagedResult.Data, Has.Count.EqualTo(3));
                Assert.That(pagedResult.TotalRecords, Is.EqualTo(5));
                Assert.That(pagedResult.CurrentPage, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task SearchClients_ShouldReturnMatchingResults()
        {
            await _clientService.CreateClient(new CreateClientRequest
            {
                Name = "Eva Long",
                DateOfBirth = new DateTime(1992, 10, 10),
                Gender = "Female",
                PhoneNumber = "8111222233",
                Email = "eva@example.com",
                AadhaarNumber = "777766665555",
                PANNumber = "EVALO1234L",
                Address = "Paradise City",
                Password = "EvaPassword"
            });

            var matches = await _clientService.SearchClients("Eva");

            Assert.Multiple(() =>
            {
                Assert.That(matches, Is.Not.Null);
                Assert.That(matches, Is.Not.Empty);
                Assert.That(matches.First().Email, Is.EqualTo("eva@example.com"));
            });
        }

        [Test]
        public void ChangeClientPassword_ShouldFailWithIncorrectOldPassword()
        {
            var request = new CreateClientRequest
            {
                Name = "Bruce Wayne",
                DateOfBirth = new DateTime(1975, 2, 19),
                Gender = "Male",
                PhoneNumber = "9876543210",
                Email = "bruce@wayneenterprises.com",
                AadhaarNumber = "999911112222",
                PANNumber = "BRUCE1234W",
                Address = "Gotham",
                Password = "DarkKnight1"
            };

            Assert.DoesNotThrowAsync(async () => await _clientService.CreateClient(request));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _clientService.ChangeClientPassword("bruce@wayneenterprises.com", "WrongPassword", "NewPass123"));

            Assert.That(ex.Message, Is.EqualTo("Old password is incorrect"));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
