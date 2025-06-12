using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class InsuranceController : ControllerBase
    {
        private readonly IInsuranceService _insuranceService;
        private readonly IPolicyDocumentService _policyDocumentService;
      

        public InsuranceController(IInsuranceService insuranceService, IPolicyDocumentService policyDocumentService)
        {
            _insuranceService = insuranceService;
            _policyDocumentService = policyDocumentService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("Admin/GenerateInsurance/{proposalId}")]
        public async Task<ActionResult<InsuranceResponse>> GenerateInsurance(int proposalId)
        {
            try
            {
                var insurance = await _insuranceService.GenerateInsuranceAsync(proposalId);
                return Ok(insurance);
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
            
        [Authorize(Roles = "Admin")]
        [HttpPost("Admin/Generatequote/{proposalId}")]
        public async Task<ActionResult<InsuranceQuoteResponse>> GenerateQuote(int proposalId)
        {
            try
            {
                var quote = await _insuranceService.GenerateQuote(proposalId);
                return Ok(quote);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("Admin/GetPaidProposal")]
        public async Task<ActionResult<IEnumerable<ProposalReviewDto>>> GetPaidProposals()
        {
            try
            {
                var proposals = await _insuranceService.GetPaidProposals();
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

        [Authorize(Roles = "Client")]
        [HttpGet("client/proposal/{proposalId}")]
        public async Task<IActionResult> GetInsuranceByProposalIdForClient(int proposalId)
        {
            try
            {
                var result = await _insuranceService.GetInsuranceByProposalIdForClientAsync(proposalId, User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Optionally log the error here
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }
        [Authorize(Roles = "Client")]
        [HttpGet("Client/ViewPolicy")]
        public async Task<IActionResult> GetAllInsurancesForClient()
        {
            try
            {
                var result = await _insuranceService.GetAllInsurancesForClientAsync(User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllInsurancesForAdmin([FromQuery] int page = 1, [FromQuery] int pageSize = 7)
        {
            try
            {
                var result = await _insuranceService.GetAllInsurancesForAdminAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Optionally log the error
                return StatusCode(500, new { message = "An error occurred while fetching insurances.", details = ex.Message });
            }
        }

        [Authorize(Roles = "Client")]
        [HttpGet("Client/TrackStatus")]
        public async Task<IActionResult> TrackMyPolicies()
        {
            var clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (clientIdClaim == null)
                return Unauthorized("Client ID not found in token.");

            if (!int.TryParse(clientIdClaim.Value, out int clientId))
                return Unauthorized("Invalid Client ID in token.");

            var result = await _insuranceService.GetClientPolicyStatusAsync(clientId);

            return Ok(result ?? new List<ClientPolicyStatusDto>());
        }

        [HttpGet("Client/Download-policy/{insurancePolicyNumber}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> DownloadPolicyDocument(string insurancePolicyNumber)
        {
            var insurance = await _insuranceService.GetInsuranceWithDetailsAsync(insurancePolicyNumber);
            if (insurance == null || insurance.Status != "active")
                return NotFound("Insurance policy not found or not active.");

            var pdfBytes = _policyDocumentService.GeneratePolicyDocument(insurance);

            return File(pdfBytes, "application/pdf", $"Policy_{insurancePolicyNumber}.pdf");
        }



    }
}
