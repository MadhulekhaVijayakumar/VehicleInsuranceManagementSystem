using InsuranceAPI.Models.DTOs;

namespace InsuranceAPI.Interfaces
{
    public interface IAdminService
    {
        Task<CreateAdminResponse> CreateAdmin(CreateAdminRequest request);
    }
}
