using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace InsuranceAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<int, Admin> _adminRepository;

        public AdminService(IRepository<string, User> userRepository, IRepository<int, Admin> adminRepository)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
        }

        public async Task<CreateAdminResponse> CreateAdmin(CreateAdminRequest request)
        {
            HMACSHA512 hmac = new HMACSHA512();
            byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

            var user = new User
            {
                Username = request.Email,
                Password = passwordHash,
                HashKey = hmac.Key,
                Role = "Admin"
            };

            var userResult = await _userRepository.Add(user);
            if (userResult == null)
                throw new Exception("Failed to create user");

            var admin = new Admin
            {
                Name = request.Name,
                Email = request.Email
            };

            var adminResult = await _adminRepository.Add(admin);
            if (adminResult == null)
                throw new Exception("Failed to create admin");

            return new CreateAdminResponse
            {
                Id = adminResult.Id,
                Message = "Admin created successfully"
            };

        }
    }
}
