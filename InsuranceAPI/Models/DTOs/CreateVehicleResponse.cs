namespace InsuranceAPI.Models.DTOs
{
    public class CreateVehicleResponse
    {
        public int VehicleId { get; set; }
        public string Message { get; set; } = "Vehicle registered successfully";
    }
}
