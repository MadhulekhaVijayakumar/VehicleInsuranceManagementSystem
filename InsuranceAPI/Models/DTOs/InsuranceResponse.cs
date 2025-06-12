namespace InsuranceAPI.Models.DTOs
{
    public class InsuranceResponse
    {
        public string InsurancePolicyNumber { get; set; } = string.Empty;
        public int ProposalId { get; set; }
        public int VehicleId { get; set; }
        public int ClientId { get; set; }
        public decimal PremiumAmount { get; set; }
        public DateTime InsuranceStartDate { get; set; }
        public decimal InsuranceSum { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
