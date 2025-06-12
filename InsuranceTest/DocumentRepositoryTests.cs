using InsuranceAPI.Models;
using InsuranceAPI.Context;
using InsuranceAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace InsuranceAPI.Tests.Repositories
{
    [TestFixture]
    public class DocumentRepositoryTests
    {
        private InsuranceManagementContext _context;
        private DocumentRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new InsuranceManagementContext(options);

            // Seed document
            var document = new Document
            {
                DocumentId = 1,
                ProposalId = 1,
                FileName = "license.pdf",
                FileType = "application/pdf",
                Data = new byte[] { 1, 2, 3 }
            };

            _context.Documents.Add(document);
            _context.SaveChanges();

            _repository = new DocumentRepository(_context);
        }

        [Test]
        public async Task GetById_ValidId_ReturnsDocument()
        {
            var doc = await _repository.GetById(1);
            Assert.IsNotNull(doc);
            Assert.AreEqual("license.pdf", doc.FileName);
        }

        [Test]
        public void GetById_InvalidId_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetById(999));
            Assert.That(ex.Message, Is.EqualTo("Document with ID 999 not found"));
        }

        [Test]
        public async Task GetAll_ReturnsAllDocuments()
        {
            var docs = await _repository.GetAll();
            Assert.AreEqual(1, ((List<Document>)docs).Count);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
