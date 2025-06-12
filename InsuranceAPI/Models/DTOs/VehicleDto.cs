namespace InsuranceAPI.Models.DTOs
{
    public class VehicleDto
    {
        public int VehicleId { get; set; }
       
        public string VechileType { get; set; }= string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public string ChassisNumber { get; set; } = string.Empty;
        public string EngineNumber { get; set; } = string.Empty;
        public string MakerName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string VehicleColor { get; set; } = string.Empty;
        public string FuelType { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public int SeatCapacity { get; set; }
    }
}
