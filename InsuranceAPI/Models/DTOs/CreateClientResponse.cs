namespace InsuranceAPI.Models.DTOs
{
    public class CreateClientResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = "Client created successfully";
    }
}
