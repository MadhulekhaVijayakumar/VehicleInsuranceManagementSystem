namespace InsuranceAPI.Models.DTOs
{
    public class ClaimSummaryDto
    {
        public int ClaimId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public DateTime IncidentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

}
