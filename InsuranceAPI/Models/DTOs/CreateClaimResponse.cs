namespace InsuranceAPI.Models.DTOs
{
    public class CreateClaimResponse
    {
        public int ClaimId { get; set; }
        public string Status { get; set; } = "Pending";
       public string? Message { get; set; }
    }
}
