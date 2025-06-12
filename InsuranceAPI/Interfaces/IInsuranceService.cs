using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using System.Security.Claims;

namespace InsuranceAPI.Interfaces
{
    public interface IInsuranceService
    {
        Task<InsuranceResponse> GenerateInsuranceAsync(int proposalId);
        Task<InsuranceResponseDto> GetInsuranceByProposalIdForClientAsync(int proposalId, ClaimsPrincipal user);
        Task<List<InsuranceResponseDto>> GetAllInsurancesForClientAsync(ClaimsPrincipal user);
        Task<PaginatedResult<InsuranceResponseDto>> GetAllInsurancesForAdminAsync(int page, int pageSize);
        Task<IEnumerable<ClientPolicyStatusDto>> GetClientPolicyStatusAsync(int clientId);
        Task<InsuranceQuoteResponse> GenerateQuote(int proposalId);
        Task<Insurance> GetInsuranceWithDetailsAsync(string policyNumber);
        Task<IEnumerable<ProposalReviewDto>> GetPaidProposals();
    }

}
