namespace InsuranceAPI.Models.DTOs
{
    public class ProposalReviewDto
    {
        public int ClientId { get; set; }
        public string? ClientName { get; set; }
        public int ProposalId { get; set; }
        public string? InsuranceType { get; set; }
        public decimal? InsuranceSum { get; set; }
        public DateTime? InsuranceStartDate { get; set; }
        public decimal? CalculatedPremium { get; set; }
        public string? Status { get; set; }
    }
}
