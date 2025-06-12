using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Repositories;
using System.Security.Cryptography;
namespace InsuranceAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository<string, User> _userRepository;
        private IRepository<int, Client> _clientRepository;
        private readonly ITokenService _tokenService;
        private readonly IRepository<int,Admin> _adminRepository;

        public AuthenticationService(IRepository<string, User> userRpository,
                                     IRepository<int, Client> clientRepository,
                                     IRepository<int,Admin> adminrRepository,
                                     ITokenService tokenService)
        {
            _userRepository = userRpository;
            _clientRepository = clientRepository;
            _adminRepository = adminrRepository;
            _tokenService = tokenService;
        }
        public async Task<LoginResponse> Login(UserLoginRequest loginRequest)
        {
            var user = await _userRepository.GetById(loginRequest.Username);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            HMACSHA512 hmac = new HMACSHA512(user.HashKey);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginRequest.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.Password[i])
                    throw new UnauthorizedAccessException("Invalid password");
            }

            string name = "";
            int id = 0;
            string email = "";

            if (user.Role == "Client")
            {
                var client = (await _clientRepository.GetAll()).FirstOrDefault(c => c.Email == loginRequest.Username);
                if (client == null)
                    throw new UnauthorizedAccessException("Client not found");
                name = client.Name;
                id = client.Id;
                email=client.Email;
            }
            else if (user.Role == "Admin")
            {

                var admin = (await _adminRepository.GetAll()).FirstOrDefault(a => a.Email == loginRequest.Username);
                if (admin == null)
                    throw new UnauthorizedAccessException("Admin not found");
                name = admin.Name;
                id = admin.Id;
                email = admin.Email;
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid role");
            }

            var token = await _tokenService.GenerateToken(id, name, user.Role,email);
            return new LoginResponse { Id = id, Name = name, Role = user.Role, Token = token };
        }

    }
}

