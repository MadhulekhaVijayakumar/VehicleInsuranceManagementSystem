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
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        //for sign up purpose
        [HttpPost("Register")]

        public async Task<ActionResult<CreateClientResponse>> CreateClient(CreateClientRequest request)
        {
            try
            {
                var result = await _clientService.CreateClient(request);
                return Created("", result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("Profile")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<ClientProfileResponse>> GetProfile()
        {
            var clientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (clientId == null) return Unauthorized();

            var result = await _clientService.GetClientProfile(int.Parse(clientId));
            return Ok(result);
        }

        [HttpPut("Update-profile")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<ClientProfileResponse>> UpdateProfile([FromBody] UpdateClientRequest request)
        {
            try
            {
                // Extract Client ID from token
                var clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (clientIdClaim == null)
                    return Unauthorized("Client ID not found in token");

                int clientId = int.Parse(clientIdClaim.Value);

                var updatedProfile = await _clientService.UpdateClientProfile(clientId, request);
                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Client")]
        [HttpPost("Change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                // Extract the email (used as username) from the JWT
                var email = User.FindFirstValue(ClaimTypes.Email); // or ClaimTypes.Email based on your JWT claims

                if (string.IsNullOrEmpty(email))
                    return Unauthorized("Invalid token or email not found.");

                var result = await _clientService.ChangeClientPassword(email, request.OldPassword, request.NewPassword);

                if (!result)
                    return BadRequest("Password change failed.");

                return Ok("Password changed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("AllClient")]
        public async Task<IActionResult> GetAllClients([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _clientService.GetAllClients(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework here)
                return StatusCode(500, new { message = "An error occurred while fetching clients.", error = ex.Message });
            }
        }





    }

}
