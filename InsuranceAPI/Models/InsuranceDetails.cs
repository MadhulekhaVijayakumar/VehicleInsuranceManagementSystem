using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAPI.Models
{
    public class InsuranceDetails
    {
        [Key]
        public int Id { get; set; }

        public int ProposalId { get; set; }
        public Proposal? Proposal { get; set; }

        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        [Required, Column(TypeName = "date")]
        public DateTime InsuranceStartDate { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal InsuranceSum { get; set; }

        [Required, MaxLength(50)]
        public string DamageInsurance { get; set; } = string.Empty; // No / Partial / Full

        [Required, MaxLength(50)]
        public string LiabilityOption { get; set; } = string.Empty; // Third-Party or Own Damage

        [Required, MaxLength(20)]
        public string Plan { get; set; } = string.Empty; // Silver, Gold, Platinum
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal CalculatedPremium { get; set; }

    }
}
