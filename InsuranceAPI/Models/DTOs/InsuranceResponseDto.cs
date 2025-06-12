namespace InsuranceAPI.Models.DTOs
{
    public class InsuranceResponseDto
    {
        public string InsurancePolicyNumber { get; set; }=string.Empty;
        public string VehicleNumber { get; set; }= string.Empty;
        public string Name { get; set; } = string.Empty;
        public string InsuranceType { get; set; } = string.Empty;
        public string Plan { get; set; }= string.Empty;
        public DateTime InsuranceStartDate { get; set; }
        public decimal InsuranceSum { get; set; }
        public decimal PremiumAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
    }

}
