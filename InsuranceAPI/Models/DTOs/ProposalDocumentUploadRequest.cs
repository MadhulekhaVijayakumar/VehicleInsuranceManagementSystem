using System.Text.Json.Serialization;

namespace InsuranceAPI.Models.DTOs
{
    public class ProposalDocumentUploadRequest
    {
        [JsonIgnore] // This hides it from Swagger and prevents client from sending it
        public int ProposalId { get; set; }

        public IFormFile? License { get; set; }
        public IFormFile? RCBook { get; set; }
        public IFormFile? PollutionCertificate { get; set; }
    }
}
