namespace InsuranceAPI.Models.DTOs
{
    public class PaymentStatusDto
    {
        public int ProposalId { get; set; }
        public string TransactionStatus { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
