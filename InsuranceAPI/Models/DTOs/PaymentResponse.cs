using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAPI.Models.DTOs
{
    public class PaymentResponse
    {
        public int PaymentId { get; set; }
        public int ProposalId { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = "success"; // or Pending, Failed
        public string ErrorMessage { get; internal set; }
    }
}
