namespace InsuranceAPI.Models.DTOs
{
    public class InsuranceQuoteResponse
    {
        public int ProposalId { get; set; }
        public decimal PremiumAmount { get; set; }
        public decimal InsuranceSum { get; set; }
        public DateTime InsuranceStartDate { get; set; }
        public string Status { get; set; }
    }
}
