using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Repositories;

namespace InsuranceAPI.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IRepository<int, Document> _documentRepository;
        private readonly ILogger<DocumentService> _logger;

        private readonly List<string> _allowedExtensions = new() { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
        private const long _maxFileSize = 5 * 1024 * 1024;

        public DocumentService(IRepository<int, Document> documentRepository, ILogger<DocumentService> logger)
        {
            _documentRepository = documentRepository;
            _logger = logger;
        }

        public async Task SaveDocumentsAsync(ProposalDocumentUploadRequest request)
        {
            var documents = new List<Document>();
            var validationErrors = new List<string>();

            async Task AddIfValid(IFormFile file, string fileType)
            {
                if (file != null)
                {
                    var result = await TryConvertToDocumentAsync(file, fileType, request.ProposalId);
                    if (result.Item1 != null)
                        documents.Add(result.Item1);
                    else
                        validationErrors.Add($"{fileType}: {result.Item2}");
                }
            }

            await AddIfValid(request.License, "License");
            await AddIfValid(request.RCBook, "RC Book");
            await AddIfValid(request.PollutionCertificate, "Pollution Certificate");

            if (validationErrors.Any())
            {
                _logger.LogWarning("Upload validation failed: {Errors}", string.Join(" | ", validationErrors));
                throw new Exception("Document upload failed:\n" + string.Join("\n", validationErrors));
            }

            foreach (var doc in documents)
            {
                await _documentRepository.Add(doc);
            }
        }

        private async Task<(Document?, string)> TryConvertToDocumentAsync(IFormFile file, string fileType, int proposalId)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!_allowedExtensions.Contains(extension))
                return (null, $"Invalid file type {extension}. Only PDF, DOC, DOCX, JPG, PNG allowed.");

            if (file.Length > _maxFileSize)
                return (null, $"File {file.FileName} exceeds 5MB size limit.");

            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);

                return (new Document
                {
                    FileName = file.FileName,
                    FileType = fileType,
                    Data = ms.ToArray(),
                    ProposalId = proposalId
                }, null);
            }
            catch (Exception ex)
            {
                return (null, $"Error reading file {file.FileName}: {ex.Message}");
            }
        }
        public async Task SaveClaimDocumentsAsync(int claimId, ClaimDocumentUploadRequest request)
        {
            var documents = new List<Document>();
            var validationErrors = new List<string>();

            async Task AddIfValid(IFormFile file, string fileType)
            {
                if (file != null)
                {
                    var result = await TryConvertClaimDocumentAsync(file, fileType, claimId);
                    if (result.Item1 != null)
                        documents.Add(result.Item1);
                    else
                        validationErrors.Add($"{fileType}: {result.Item2}");
                }
            }

            await AddIfValid(request.RepairEstimateCost, "RepairEstimateCost");
            await AddIfValid(request.AccidentCopy, "AccidentCopy");
            await AddIfValid(request.FIRCopy, "FIRCopy");

            if (validationErrors.Any())
            {
                _logger.LogWarning("Claim document upload failed: {Errors}", string.Join(" | ", validationErrors));
                throw new Exception("Document upload failed:\n" + string.Join("\n", validationErrors));
            }

            foreach (var doc in documents)
            {
                await _documentRepository.Add(doc);
            }
        }

        private async Task<(Document?, string)> TryConvertClaimDocumentAsync(IFormFile file, string fileType, int claimId)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!_allowedExtensions.Contains(extension))
                return (null, $"Invalid file extension {extension}. Allowed: {string.Join(", ", _allowedExtensions)}");

            if (file.Length > _maxFileSize)
                return (null, $"File {file.FileName} exceeds the 5MB limit.");

            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);

                return (new Document
                {
                    FileName = file.FileName,
                    FileType = fileType,
                    Data = ms.ToArray(),
                    ClaimId = claimId
                }, null);
            }
            catch (Exception ex)
            {
                return (null, $"Error processing {file.FileName}: {ex.Message}");
            }
        }


        public async Task<(byte[] FileData, string FileName)?> DownloadClaimDocumentAsync(int claimId, string fileType)
        {
            var allDocuments = await _documentRepository.GetAll();

            var document = allDocuments
                .FirstOrDefault(d => d.ClaimId == claimId && d.FileType.ToLower() == fileType.ToLower());

            if (document == null)
                return null;

            return (document.Data, document.FileName);
        }
        public async Task<(byte[] FileData, string FileName)?> DownloadProposalDocumentAsync(int proposalId, string fileType)
        {
            var allDocuments = await _documentRepository.GetAll();

            var document = allDocuments
                .FirstOrDefault(d => d.ProposalId == proposalId && d.FileType.ToLower() == fileType.ToLower());

            if (document == null)
                return null;

            return (document.Data, document.FileName);
        }


    }
}
