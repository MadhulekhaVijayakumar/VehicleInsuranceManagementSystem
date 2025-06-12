using InsuranceAPI.Models.DTOs;

namespace InsuranceAPI.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> Login(UserLoginRequest loginRequest);
    }
}
