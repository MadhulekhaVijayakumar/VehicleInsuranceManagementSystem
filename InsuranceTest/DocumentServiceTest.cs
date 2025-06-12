using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Repositories;
using InsuranceAPI.Services;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using InsuranceAPI.Interfaces;

namespace InsuranceTest
{
    [TestFixture]
    public class DocumentServiceTests
    {
        private Mock<IRepository<int, Document>> _mockDocumentRepository;
        private Mock<ILogger<DocumentService>> _mockLogger;
        private DocumentService _documentService;

        [SetUp]
        public void Setup()
        {
            _mockDocumentRepository = new Mock<IRepository<int, Document>>();
            _mockLogger = new Mock<ILogger<DocumentService>>();

            _documentService = new DocumentService(_mockDocumentRepository.Object, _mockLogger.Object);
        }

        // Helper method to mock IFormFile
        private IFormFile CreateMockFile(string fileName, string fileExtension, long fileSize)
        {
            var fileMock = new Mock<IFormFile>();
            var fileContent = new byte[fileSize];
            var stream = new MemoryStream(fileContent);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(fileSize);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.ContentType).Returns("application/octet-stream");
            return fileMock.Object;
        }

        [Test]
        public async Task SaveDocumentsAsync_ShouldSaveValidDocuments()
        {
            // Arrange
            var request = new ProposalDocumentUploadRequest
            {
                ProposalId = 1,
                License = CreateMockFile("license.pdf", ".pdf", 1024 * 500), // 500KB PDF file
                RCBook = CreateMockFile("rcbook.docx", ".docx", 1024 * 600), // 600KB DOCX file
                PollutionCertificate = CreateMockFile("pollution.jpg", ".jpg", 1024 * 300) // 300KB JPG file
            };

            var documents = new List<Document>
            {
                new Document { FileName = "license.pdf", FileType = "License", Data = new byte[500] },
                new Document { FileName = "rcbook.docx", FileType = "RC Book", Data = new byte[600] },
                new Document { FileName = "pollution.jpg", FileType = "Pollution Certificate", Data = new byte[300] }
            };

            _mockDocumentRepository
     .Setup(repo => repo.Add(It.IsAny<Document>()))
     .ReturnsAsync((Document doc) => doc);


            // Act
            await _documentService.SaveDocumentsAsync(request);

            // Assert
            _mockDocumentRepository.Verify(repo => repo.Add(It.IsAny<Document>()), Times.Exactly(3));
        }

        [Test]
        public async Task SaveDocumentsAsync_ShouldThrowException_WhenFileExceedsSizeLimit()
        {
            // Arrange
            var request = new ProposalDocumentUploadRequest
            {
                ProposalId = 1,
                License = CreateMockFile("license.pdf", ".pdf", 1024 * 6000) // 6MB PDF file exceeding limit
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _documentService.SaveDocumentsAsync(request));
            Assert.That(ex.Message, Does.Contain("exceeds 5MB size limit"));
        }

        [Test]
        public async Task SaveDocumentsAsync_ShouldThrowException_WhenFileHasInvalidExtension()
        {
            // Arrange
            var request = new ProposalDocumentUploadRequest
            {
                ProposalId = 1,
                License = CreateMockFile("license.exe", ".exe", 1024 * 400) // Invalid file type
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _documentService.SaveDocumentsAsync(request));
            Assert.That(ex.Message, Does.Contain("Invalid file type .exe"));
        }

        [Test]
        public async Task SaveDocumentsAsync_ShouldThrowException_WhenErrorReadingFile()
        {
            // Arrange
            var request = new ProposalDocumentUploadRequest
            {
                ProposalId = 1,
                License = CreateMockFile("license.pdf", ".pdf", 1024 * 500)
            };

            // Simulate an error in file reading (force exception in TryConvertToDocumentAsync)
            _mockDocumentRepository.Setup(repo => repo.Add(It.IsAny<Document>())).ThrowsAsync(new Exception("Error processing file"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _documentService.SaveDocumentsAsync(request));
            StringAssert.Contains("Error processing file", ex.Message);

        }

        [Test]
        public async Task DownloadClaimDocumentAsync_ShouldReturnDocument_WhenFound()
        {
            // Arrange
            var claimId = 1;
            var fileType = "RepairEstimateCost";
            var document = new Document
            {
                ClaimId = claimId,
                FileType = fileType,
                Data = new byte[100],
                FileName = "repair_estimate.pdf"
            };
            _mockDocumentRepository.Setup(repo => repo.GetAll()).ReturnsAsync(new List<Document> { document });

            // Act
            var result = await _documentService.DownloadClaimDocumentAsync(claimId, fileType);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("repair_estimate.pdf", result?.FileName);
        }

        [Test]
        public async Task DownloadClaimDocumentAsync_ShouldReturnNull_WhenDocumentNotFound()
        {
            // Arrange
            var claimId = 1;
            var fileType = "NonExistentFileType";
            _mockDocumentRepository.Setup(repo => repo.GetAll()).ReturnsAsync(new List<Document>());

            // Act
            var result = await _documentService.DownloadClaimDocumentAsync(claimId, fileType);

            // Assert
            Assert.IsNull(result);
        }
    }
}
