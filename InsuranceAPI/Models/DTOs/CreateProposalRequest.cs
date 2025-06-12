using System.ComponentModel.DataAnnotations;

namespace InsuranceAPI.Models.DTOs
{
    public class CreateProposalRequest
    {
        public CreateVehicleRequest? Vehicle { get; set; }
        public CreateProposalData? Proposal { get; set; }
        public CreateInsuranceDetailRequest? InsuranceDetails { get; set; }
        public ProposalDocumentUploadRequest? Documents { get; set; }

    }

}
