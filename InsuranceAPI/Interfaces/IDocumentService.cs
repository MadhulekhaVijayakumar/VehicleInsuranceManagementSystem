using InsuranceAPI.Models.DTOs;

namespace InsuranceAPI.Interfaces
{
    public interface IDocumentService
    {
        Task SaveDocumentsAsync(ProposalDocumentUploadRequest request);
        Task SaveClaimDocumentsAsync(int claimId, ClaimDocumentUploadRequest request);
        Task<(byte[] FileData, string FileName)?> DownloadClaimDocumentAsync(int claimId, string fileType);
        Task<(byte[] FileData, string FileName)?> DownloadProposalDocumentAsync(int proposalId, string fileType);


    }
}
