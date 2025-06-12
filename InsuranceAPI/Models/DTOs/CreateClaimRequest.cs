namespace InsuranceAPI.Models.DTOs
{
    public class CreateClaimRequest
    {
        public string InsurancePolicyNumber { get; set; } = string.Empty;
        public DateTime IncidentDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal? ClaimAmount { get; set; }

        public ClaimDocumentUploadRequest? Documents { get; set; }
    }
}
