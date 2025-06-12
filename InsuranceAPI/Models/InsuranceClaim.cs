using System.Text.Json.Serialization;

namespace InsuranceAPI.Models
{
    public class InsuranceClaim
    {
        public int ClaimId { get; set; }
        public string InsurancePolicyNumber { get; set; } = string.Empty;
        public DateTime IncidentDate { get; set; }
        public decimal? ClaimAmount { get; set; } 
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public Insurance? Insurance { get; set; }
        public ICollection<Document>? Documents { get; set; }

    }
}
