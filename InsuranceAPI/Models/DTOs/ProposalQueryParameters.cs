namespace InsuranceAPI.Models.DTOs
{
    public class ProposalQueryParameters
    {
        public int Page { get; set; } = 1; // Default page = 1
        public int PageSize { get; set; } = 10; // Default page size = 10
        public string? Status { get; set; } // Optional filter
        public string? Search { get; set; } // Optional search (Client Name / Vehicle Number)
    }

}
