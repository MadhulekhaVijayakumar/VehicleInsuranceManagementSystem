using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Client")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }



        [HttpGet("Client/GetVehicle")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetMyVehicles()
        {
            var clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out int clientId))
            {
                return Unauthorized("Client ID not found in token.");
            }

            var vehicles = await _vehicleService.GetAllVehiclesByClient(clientId);
            return Ok(vehicles);
        }



    }
}
