using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAPI.Models.DTOs
{
    public class CreatePaymentRequest
    {
        public int ProposalId { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountPaid { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
    }
}
