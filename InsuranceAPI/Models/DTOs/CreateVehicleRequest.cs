using System.ComponentModel.DataAnnotations;

namespace InsuranceAPI.Models.DTOs
{
    public class CreateVehicleRequest
    {
        [Required]
 
        public string VehicleType { get; set; }=string.Empty;

        [Required, MaxLength(20)]
        public string VehicleNumber { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string ChassisNumber { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string EngineNumber { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string MakerName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string ModelName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string VehicleColor { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string FuelType { get; set; } = string.Empty;

        [Required]
        public DateTime RegistrationDate { get; set; }

        [Required]
        public int SeatCapacity { get; set; }
    }
}
