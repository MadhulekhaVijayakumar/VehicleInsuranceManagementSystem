namespace InsuranceAPI.Interfaces
{
    public interface IQuoteService
    {
        Task<byte[]> GenerateQuotePdfAsync(int proposalId);
    }

}
