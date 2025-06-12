using InsuranceAPI.Interfaces;
using InsuranceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private readonly IQuoteService _quoteService;

        public QuoteController(IQuoteService quoteService) 
        { 
            _quoteService=quoteService;
        }
        [Authorize(Roles = "Client,Admin")]
        [HttpGet("download-quote/{proposalId}")]
        public async Task<IActionResult> DownloadQuote(int proposalId)
        {
            try
            {
                var pdfBytes = await _quoteService.GenerateQuotePdfAsync(proposalId);
                string fileName = $"QUOTE-{proposalId:D5}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
