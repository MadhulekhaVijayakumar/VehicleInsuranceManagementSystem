using InsuranceAPI.Models;

namespace InsuranceAPI.Interfaces
{
    public interface IQuotePdfGenerator
    {
        byte[] GenerateQuotePdf(string quoteNumber, Client client, Vehicle vehicle, Proposal proposal, InsuranceDetails insurance, List<string> addOns = null);
    }

}
