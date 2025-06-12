using InsuranceAPI.Models;

namespace InsuranceAPI.Interfaces
{
    public interface IPolicyDocumentService
    {
        byte[] GeneratePolicyDocument(Insurance insurance);
    }
}
