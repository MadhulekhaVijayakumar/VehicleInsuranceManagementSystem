using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IProposalService _proposalService;
        private readonly IClientService _clientService;

        public AdminController(IAdminService adminService, IProposalService proposalService, IClientService clientService)
        {
            _adminService = adminService;
            _proposalService = proposalService;
            _clientService = clientService;
        }

        // 1. Register new Admin
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<CreateAdminResponse>> CreateAdmin(CreateAdminRequest request)
        {
            try
            {
                var result = await _adminService.CreateAdmin(request);
                return Created("", result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("SearchClient")]
        public async Task<ActionResult<IEnumerable<ClientProfileResponse>>> SearchClients([FromQuery] string keyword)
        {
            var results = await _clientService.SearchClients(keyword);
            return Ok(results);
        }
       


    }
}
