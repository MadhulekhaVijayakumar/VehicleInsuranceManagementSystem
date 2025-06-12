namespace InsuranceAPI.Models.DTOs
{
    public class ClaimReviewDto
    {
        // Insurance
        public string InsurancePolicyNumber { get; set; } = string.Empty;
        public DateTime InsuranceStartDate { get; set; }
        public decimal InsuranceSum { get; set; }
        public decimal PremiumAmount { get; set; }
        public string InsuranceStatus { get; set; } = string.Empty;

        // Client
        public string ClientName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }=string.Empty;
        public string Address { get; set; }=string.Empty;

        // Vehicle
        public string VehicleNumber { get; set; }=string.Empty;
        public string VehicleType { get; set; }= string.Empty;
        public string Make { get; set; }= string.Empty;
        public string Model { get; set; }= string.Empty;
        public DateTime RegistrationDate{ get; set; }

        // Claim
        public int ClaimId { get; set; }
        public DateTime IncidentDate { get; set; }
        public decimal ClaimAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;


        // Documents
        public ClaimDocumentFileNames? Documents { get; set; }
    }

}
