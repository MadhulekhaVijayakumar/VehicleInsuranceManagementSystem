namespace InsuranceAPI.Models.DTOs
{
    public class ProposalDetailsResponse
    {
        public CreateVehicleRequest Vehicle { get; set; }
        public CreateProposalData Proposal { get; set; }
        public CreateInsuranceDetailRequest InsuranceDetails { get; set; }

        public ProposalDocumentFileNames Documents { get; set; }

        public int ClientId { get; set; }
    }
}
