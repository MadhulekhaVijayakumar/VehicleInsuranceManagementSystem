using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAPI.Models
{
    public class Proposal
    {
        [Key]
        public int ProposalId { get; set; }

        // Foreign key to Client
        [Required]
        public int ClientId { get; set; }

        public Client? Client { get; set; }

        // Foreign key to Vehicle
        [Required]
        public int VehicleId { get; set; }

        public Vehicle? Vehicle { get; set; }

        [Required]
        [MaxLength(50)]
        public string InsuranceType { get; set; } = string.Empty; // e.g. Comprehensive, Third-Party

        [Required]
        [Column(TypeName = "date")]
        public DateTime? InsuranceValidUpto { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime FitnessValidUpto { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "submitted"; // submitted, quote generated, active, expired, rejected

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public InsuranceDetails? InsuranceDetails { get; set; }
        public Insurance? Insurance { get; set; }
        public IEnumerable<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Document> Documents { get; set; }


    }
}
