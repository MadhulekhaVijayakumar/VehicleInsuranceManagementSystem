namespace InsuranceAPI.Models.DTOs
{
    public class CreateInsuranceDetailRequest
    {
        public DateTime InsuranceStartDate { get; set; }
        public decimal InsuranceSum { get; set; }
        public string DamageInsurance { get; set; }
        public string LiabilityOption { get; set; }
        public string Plan { get; set; }
    }
}
