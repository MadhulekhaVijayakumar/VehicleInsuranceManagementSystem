namespace InsuranceAPI.Models.DTOs
{
    public class AdminDashboardSummaryDto
    {
        public int TotalClients { get; set; }
        public int PendingProposals { get; set; }
        public int ClaimsToReview { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
