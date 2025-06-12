using System.ComponentModel.DataAnnotations;

namespace InsuranceAPI.Models.DTOs
{
    public class CreateInsuranceRequest
    {
        public int ProposalId { get; set; }
        public int VehicleId { get; set; }
        public int ClientId { get; set; }

        
    }
}
