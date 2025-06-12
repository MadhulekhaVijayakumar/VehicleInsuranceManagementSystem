using InsuranceAPI.Models.DTOs;

namespace InsuranceAPI.Interfaces
{
    public interface IPaymentService
    {
        Task<object> ProcessPaymentAsync(CreatePaymentRequest request);
    }
}