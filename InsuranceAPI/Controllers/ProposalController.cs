using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProposalController : ControllerBase
    {
        private readonly IProposalService _proposalService;
        private readonly IDocumentService _documentService;

        public ProposalController(IProposalService proposalService,IDocumentService documentService)
        {
            _proposalService = proposalService;
            _documentService = documentService;
        }
        [HttpPost("SubmitProposal")]
        [Authorize(Roles = "Client")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<CreateProposalResponse>> SubmitProposal([FromForm] CreateProposalRequest request)
        {
            try
            {
                var response = await _proposalService.SubmitProposalWithDetails(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to submit proposal: {ex.Message}");
            }
        }

        [HttpGet("Admin/GetSubmitted")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProposalReviewDto>>> GetSubmittedProposals()
        {
            try
            {
                var proposals = await _proposalService.GetSubmittedProposals();
                return Ok(proposals);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve submitted proposals: {ex.Message}");
            }

        }

        [HttpGet("GetProposalDetails/{proposalId}")]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> GetProposalDetailsByProposalId(int proposalId)
        {
            try
            {
                var result = await _proposalService.GetProposalDetailsByProposalIdAsync(proposalId);

                if (result == null)
                    return NotFound(new { message = "Proposal not found." });

                // ✅ Optional: Restrict clients from accessing other clients' proposals
                var userRole = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                if (userRole == "Client")
                {
                    var clientIdFromToken = User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value;
                    if (!string.IsNullOrEmpty(clientIdFromToken) && int.TryParse(clientIdFromToken, out var clientId))
                    {
                        if (result.ClientId != clientId)
                        {
                            return Forbid("You are not authorized to access this proposal.");
                        }
                    }
                    else
                    {
                        return Unauthorized("Client ID missing or invalid.");
                    }
                }


                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the proposal details.", error = ex.Message });
            }
        }


        [HttpGet("Admin/Download-document")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownloadProposalDocument([FromQuery] int proposalId, [FromQuery] string fileType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileType))
                    return BadRequest(new { message = "File type must be provided." });

                var result = await _documentService.DownloadProposalDocumentAsync(proposalId, fileType);

                if (result == null)
                    return NotFound(new { message = "Document not found for the specified proposal and file type." });

                var (fileData, fileName) = result.Value;
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".pdf" => "application/pdf",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };


                return File(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                // Log the exception if logging is available
                return StatusCode(500, new { message = "An error occurred while downloading the document.", error = ex.Message });
            }
        }




        // 2. Approve or Reject a proposal
        [HttpPut("Admin/Verify/{proposalId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyProposal(int proposalId, [FromQuery] bool approve)
        {
            var success = await _proposalService.VerifyProposal(proposalId, approve);
            if (!success)
                return BadRequest("Invalid proposal ID or status is not 'submitted'");

            return Ok(new { message = approve ? "Proposal approved." : "Proposal rejected." });
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin")] // Only Admin can access this route
        public async Task<IActionResult> GetProposalsPagedAsync([FromQuery] ProposalQueryParameters queryParameters)
        {
            // Call service to get paged proposals
            var pagedProposals = await _proposalService.GetProposalsPagedAsync(queryParameters);

            // Return the response
            return Ok(pagedProposals);
        }
        [HttpGet("Admin/GetApproved")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProposalReviewDto>>> GetApprovedProposals()
        {
            try
            {
                var proposals = await _proposalService.GetApprovedProposals();
                return Ok(proposals);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve approved proposals: {ex.Message}");
            }

        }
        [HttpGet("Client/GetQuotes")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<IEnumerable<ProposalReviewDto>>> GetQuotesProposals()
        {
            try
            {
                var clientIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out var clientId))
                    return Unauthorized("Invalid client ID in token.");

                var proposals = await _proposalService.GetQuotesProposals(clientId);
                return Ok(proposals);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve quote proposals: {ex.Message}");
            }
        }



    }



}
