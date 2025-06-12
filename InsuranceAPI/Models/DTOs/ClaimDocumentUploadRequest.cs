namespace InsuranceAPI.Models.DTOs
{
    public class ClaimDocumentUploadRequest
    {
        //public int ClaimId { get; set; }

        public IFormFile? RepairEstimateCost { get; set; }
        public IFormFile? AccidentCopy { get; set; }
        public IFormFile? FIRCopy { get; set; }
    }
}
