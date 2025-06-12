using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Models;
using System.Security.Claims;

namespace InsuranceAPI.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IRepository<int, Vehicle> _vehicleRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VehicleService(IRepository<int, Vehicle> vehicleRepo, IHttpContextAccessor httpContextAccessor)
        {
            _vehicleRepo = vehicleRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreateVehicleResponse> RegisterVehicle(CreateVehicleRequest request)

        {

            var clientIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out int clientId))
                throw new UnauthorizedAccessException("Client ID not found in token.");
            var vehicle = new Vehicle
            {
                ClientId = clientId,
                VehicleType = request.VehicleType,
                VehicleNumber = request.VehicleNumber,
                ChassisNumber = request.ChassisNumber,
                EngineNumber = request.EngineNumber,
                MakerName = request.MakerName,
                ModelName = request.ModelName,
                VehicleColor = request.VehicleColor,
                FuelType = request.FuelType,
                RegistrationDate = request.RegistrationDate,
                SeatCapacity = request.SeatCapacity
            };

            var result = await _vehicleRepo.Add(vehicle);
            return new CreateVehicleResponse { VehicleId = result.VehicleId };
        }

        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesByClient(int clientId)
        {
            var clientIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var vehicles = await _vehicleRepo.GetAll();
            return vehicles
                .Where(v => v.ClientId == clientId)
                .Select(v => new VehicleDto
                {
                    VehicleId = v.VehicleId,
                    VechileType = v.VehicleType,
                    VehicleNumber = v.VehicleNumber,
                    ChassisNumber = v.ChassisNumber,
                    EngineNumber = v.EngineNumber,
                    MakerName = v.MakerName,
                    ModelName = v.ModelName,
                    VehicleColor = v.VehicleColor,
                    FuelType = v.FuelType,
                    RegistrationDate = v.RegistrationDate,
                    SeatCapacity = v.SeatCapacity
                });
        }

        public async Task<VehicleDto> GetVehicleById(int vehicleId)
        {
            var v = await _vehicleRepo.GetById(vehicleId);
            return new VehicleDto
            {
                VehicleId = v.VehicleId,
                VechileType= v.VehicleType,
                VehicleNumber = v.VehicleNumber,
                ChassisNumber = v.ChassisNumber,
                EngineNumber = v.EngineNumber,
                MakerName = v.MakerName,
                ModelName = v.ModelName,
                VehicleColor = v.VehicleColor,
                FuelType = v.FuelType,
                RegistrationDate = v.RegistrationDate,
                SeatCapacity = v.SeatCapacity
            };
        }
    }

}
