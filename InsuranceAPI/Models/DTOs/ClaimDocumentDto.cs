namespace InsuranceAPI.Models.DTOs
{
    public class ClaimDocumentDto
    {
        public string FileName { get; set; }
        public string FileType { get; set; } // e.g., "application/pdf", "image/jpeg"
        public byte[] FileContent { get; set; } // Store the actual file as a byte array
    }

}
