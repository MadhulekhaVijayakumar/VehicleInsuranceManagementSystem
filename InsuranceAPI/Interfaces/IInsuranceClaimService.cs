using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using System.Security.Claims;

namespace InsuranceAPI.Interfaces
{
    public interface IInsuranceClaimService
    {
        Task<CreateClaimResponse> SubmitClaimWithDocumentsAsync(CreateClaimRequest request, ClaimsPrincipal user);
        Task<IEnumerable<InsuranceClaimDto>> GetClaimsByClientAsync(ClaimsPrincipal user);
        Task<PaginatedResult<InsuranceClaimDto>> GetAllClaimsForAdminAsync(int pageNumber, int pageSize);
        Task<List<PendingClaimDto>> GetAllPendingClaimsAsync();
        Task<ClaimReviewDto> GetClaimDetailsForReviewAsync(int claimId);
        Task<InsuranceClaim> UpdateClaimStatusAsync(int claimId, string newStatus);
        Task<List<PendingClaimDto>> GetAllApprovedClaimsAsync();
        Task<int> SetApprovedClaimsToSettledAsync();
    }
}
