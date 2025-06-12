using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAPI.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }

        [Required, MaxLength(20)]
        public string VehicleType { get; set; } = string.Empty; // Car, Bike, Camper Van

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

        [Required, Column(TypeName = "date")]
        public DateTime RegistrationDate { get; set; }

        [Required]
        public int SeatCapacity { get; set; }

        public IEnumerable<Proposal>? Proposals { get; set; }
        public IEnumerable<InsuranceDetails>? InsuranceDetails { get; set; }
        public IEnumerable<Insurance>? Insurances { get; set; }

    }
}
