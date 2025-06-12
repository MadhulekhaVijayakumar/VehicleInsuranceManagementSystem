using AutoMapper;
using InsuranceAPI.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceAPI.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int ProposalId { get; set; }
        public Proposal? Proposal { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountPaid { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public string PaymentMode { get; set; } = string.Empty; // e.g., UPI, Card, Netbanking

        [Required]
        public string TransactionStatus { get; set; } = "Pending"; // or Pending, Failed
    }


}
