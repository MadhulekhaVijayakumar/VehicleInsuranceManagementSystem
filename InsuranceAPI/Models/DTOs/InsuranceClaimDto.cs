namespace InsuranceAPI.Models.DTOs
{
    public class InsuranceClaimDto
    {
        public int ClaimId { get; set; }
        public string InsurancePolicyNumber { get; set; } = string.Empty;
        public DateTime IncidentDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal? ClaimAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
