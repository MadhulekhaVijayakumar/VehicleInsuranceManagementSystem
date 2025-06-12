using InsuranceAPI.Exceptions;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsuranceClaimController : ControllerBase
    {
        private readonly IInsuranceClaimService _claimService;
        private readonly ILogger<InsuranceClaimController> _logger;
        private readonly IDocumentService _documentService;

        public InsuranceClaimController(IInsuranceClaimService claimService, ILogger<InsuranceClaimController> logger,IDocumentService documentService)
        {
            _claimService = claimService;
            _logger = logger;
            _documentService = documentService;
        }

        [Authorize(Roles = "Client")]
        [HttpPost("Client/SubmitClaim")]
        public async Task<ActionResult<CreateClaimResponse>> SubmitClaimWithDocuments([FromForm] CreateClaimRequest request)
        {
            try
            {
                var result = await _claimService.SubmitClaimWithDocumentsAsync(request, User);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new CreateClaimResponse { Message = $"Error: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Client")]
        [HttpGet("Client/MyClaims")]
        public async Task<ActionResult<IEnumerable<InsuranceClaim>>> GetMyClaims()
        {
            try
            {
                var claims = await _claimService.GetClaimsByClientAsync(User);
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("Admin/AllClaim")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<InsuranceClaimDto>>> GetAllClaims([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 7)
        {
            try
            {
                var result = await _claimService.GetAllClaimsForAdminAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (AppException ex)
            {
                // Custom application-level exception
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Generic exception fallback (optional: log the error)
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<PendingClaimDto>>> GetAllPendingClaims()
        {
            try
            {
                var claims = await _claimService.GetAllPendingClaimsAsync();
                return Ok(claims);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
        }
        [HttpGet("review/{claimId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClaimReviewDto>> GetClaimDetailsForReview(int claimId)
        {
            try
            {
                var claim = await _claimService.GetClaimDetailsForReviewAsync(claimId);
                return Ok(claim);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Admin/UpdateStatus/{claimId}")]
        public async Task<ActionResult<InsuranceClaim>> UpdateClaimStatus(int claimId, [FromQuery] string newStatus)
        {
            try
            {
                var updatedClaim = await _claimService.UpdateClaimStatusAsync(claimId, newStatus);
                return Ok(updatedClaim);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("download/{claimId}/{fileType}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownloadClaimDocument(int claimId, string fileType)
        {
            var result = await _documentService.DownloadClaimDocumentAsync(claimId, fileType);

            if (result == null)
                return NotFound("Document not found.");

            // Determine MIME type based on file extension
            string mimeType = "application/octet-stream";  // Default to binary stream

            switch (Path.GetExtension(result.Value.FileName).ToLower())
            {
                case ".png":
                    mimeType = "image/png";
                    break;
                case ".jpg":
                case ".jpeg":
                    mimeType = "image/jpeg";
                    break;
                case ".pdf":
                    mimeType = "application/pdf";
                    break;
                case ".doc":
                case ".docx":
                    mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case ".txt":
                    mimeType = "text/plain";
                    break;
                    // Add more cases for other file types as needed
            }

            return File(result.Value.FileData, mimeType, result.Value.FileName);
        }
        [HttpGet("approved")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<PendingClaimDto>>> GetAllApprovedClaims()
        {
            try
            {
                var claims = await _claimService.GetAllApprovedClaimsAsync();
                if (claims == null || claims.Count == 0)
                {
                    return Ok(new { Message = "No approved claims found." });
                }
                return Ok(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching approved claims.");
                return StatusCode(500, new { Message = "An error occurred while fetching claims.", Details = ex.Message });
            }

        }

        // PUT: api/Claim/settle-approved
        [HttpPut("settle-approved")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetApprovedClaimsToSettled()
        {
            try
            {
                // Call the service to set approved claims to settled
                var updatedCount = await _claimService.SetApprovedClaimsToSettledAsync();

                // If no claims were updated, return a custom message indicating no claims were found
                if (updatedCount == 0)
                {
                    return Ok(new { Message = "No approved claims found." });
                }

                // If claims were updated, return the count of claims marked as 'Claim Settled'
                return Ok(new { Message = $"{updatedCount} claims marked as 'Claim Settled'." });
            }
            catch (NotFoundException ex)
            {
                // Handle the NotFoundException case, if thrown
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle general exceptions, log details if needed
                return StatusCode(500, new { Message = "An error occurred while settling claims.", Details = ex.Message });
            }
        }


    }
}
