namespace InsuranceAPI.Models.DTOs
{
    public class CreateProposalResponse
    {
        public int ProposalId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal CalculatedPremium { get; set; }
    }
}
