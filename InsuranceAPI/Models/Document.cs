using System.Security.Claims;

namespace InsuranceAPI.Models
{
    public class Document
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;// "License", "RC Book", etc.
        public byte[] Data { get; set; }

        public int? ProposalId { get; set; }
        public Proposal? Proposal { get; set; }
        public int? ClaimId { get; set; }
        public InsuranceClaim? Claim { get; set; }

    }

}
