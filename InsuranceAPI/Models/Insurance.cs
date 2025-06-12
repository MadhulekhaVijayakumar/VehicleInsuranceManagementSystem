using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InsuranceAPI.Models
{
    public class Insurance
    {
        [Key]
        public string InsurancePolicyNumber { get; set; } = string.Empty;

        public int ProposalId { get; set; }
        public Proposal? Proposal { get; set; }

        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        [Required]
        public decimal PremiumAmount { get; set; }

        [Required]
        public decimal InsuranceSum { get; set; }

        [Required]
        public DateTime InsuranceStartDate { get; set; }

        public string Status { get; set; } = "active";
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public ICollection<InsuranceClaim>? Claims { get; set; }
    }
}
