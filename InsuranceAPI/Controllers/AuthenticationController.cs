using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(UserLoginRequest loginRequest)
        {
            try
            {
                var result = await _authenticationService.Login(loginRequest);

                // Role check message (optional - helpful for frontend)
                if (result.Role == "Admin")
                {
                    return Ok(new
                    {
                        Message = "Admin login successful",
                        Data = result
                    });
                }
                else if (result.Role == "Client")
                {
                    return Ok(new
                    {
                        Message = "Client login successful",
                        Data = result
                    });
                }
                else
                {
                    return BadRequest("Invalid role.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
