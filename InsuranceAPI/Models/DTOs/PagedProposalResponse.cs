namespace InsuranceAPI.Models.DTOs
{
    public class PagedProposalResponse
    {
        public IEnumerable<ProposalDto> Proposals { get; set; }
        public int TotalCount { get; set; }
    }
}
