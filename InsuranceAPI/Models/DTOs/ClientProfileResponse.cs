namespace InsuranceAPI.Models.DTOs
{
    public class ClientProfileResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AadhaarNumber { get; set; } = string.Empty;
        public string PANNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
