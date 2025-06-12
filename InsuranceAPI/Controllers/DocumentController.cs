using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }
        [HttpGet("download/{claimId}/{fileType}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownloadClaimDocument(int claimId, string fileType)
        {
            var result = await _documentService.DownloadClaimDocumentAsync(claimId, fileType);

            if (result == null)
                return NotFound("Document not found.");

            // Determine content type based on file extension
            var contentType = GetContentType(fileType);

            return File(result.Value.FileData, contentType, result.Value.FileName);
        }

        private string GetContentType(string fileType)
        {
            return fileType.ToLower() switch
            {
                "pdf" => "application/pdf",
                "jpg" => "image/jpeg",
                "jpeg" => "image/jpeg",
                "png" => "image/png",
                "doc" => "application/msword",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "txt" => "text/plain",
                _ => "application/octet-stream" // Default content type
            };
        }

        [HttpGet("download-proposal/{proposalId}/{fileType}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownloadProposalDocument(int proposalId, string fileType)
        {
            var result = await _documentService.DownloadProposalDocumentAsync(proposalId, fileType);

            if (result == null)
                return NotFound("Document not found.");

            return File(result.Value.FileData, "application/octet-stream", result.Value.FileName);
        }



    }
}
