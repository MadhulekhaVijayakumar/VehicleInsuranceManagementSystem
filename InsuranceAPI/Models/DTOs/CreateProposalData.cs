namespace InsuranceAPI.Models.DTOs
{
    public class CreateProposalData
    {
        public string InsuranceType { get; set; }
        public DateTime? InsuranceValidUpto { get; set; }
        public DateTime FitnessValidUpto { get; set; }
    }
}
