namespace InsuranceAPI.Models.DTOs
{
    public class ProposalDto
    {
        public int ProposalId { get; set; }
        public int ClientId { get; set; }
        public int VehicleId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public string InsuranceType { get; set; } = string.Empty;
        public DateTime InsuranceValidUpto { get; set; }
        public DateTime FitnessValidUpto { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
