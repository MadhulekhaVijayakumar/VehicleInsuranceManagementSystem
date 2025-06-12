using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsuranceAPI.Interfaces
{
    public interface IProposalService
    {
        Task<CreateProposalResponse> SubmitProposalWithDetails(CreateProposalRequest request);
        Task<IEnumerable<ProposalReviewDto>> GetSubmittedProposals();
        Task<ProposalDetailsResponse> GetProposalDetailsByProposalIdAsync(int proposalId);
        Task<bool> VerifyProposal(int proposalId, bool approve);
        Task<PagedProposalResponse> GetProposalsPagedAsync(ProposalQueryParameters queryParameters);
        Task<IEnumerable<ProposalReviewDto>> GetApprovedProposals();
        Task<IEnumerable<ProposalReviewDto>> GetQuotesProposals(int clientId);
    }
}
