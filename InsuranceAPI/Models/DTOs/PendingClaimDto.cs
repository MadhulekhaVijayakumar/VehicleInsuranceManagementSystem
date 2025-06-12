namespace InsuranceAPI.Models.DTOs
{
    public class PendingClaimDto
    {
        public int ClaimId { get; set; }
        public string InsurancePolicyNumber { get; set; }
        public decimal ClaimAmount { get; set; }
        public string ClaimStatus { get; set; }
    }

}
