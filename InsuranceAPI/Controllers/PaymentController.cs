using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        // Ensure no method or property is named PaymentController
        [HttpPost("make-payment")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> MakePayment([FromBody] CreatePaymentRequest request)
        {
            if (request == null || request.ProposalId <= 0)
                return BadRequest("Invalid payment request.");

            try
            {
                var result = await _paymentService.ProcessPaymentAsync(request);

                // Check result type and return appropriate response
                if (result is PaymentResponse paymentResponse)
                {
                    return Ok(paymentResponse);  // 200 OK with payment info
                }

                if (result is { } && result.GetType().GetProperty("Message") != null)
                {
                    return Conflict(result);  // 409 Conflict with message
                }

                return StatusCode(500, "Unexpected result.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Payment failed: {ex.Message}");
            }
        }



    }
}