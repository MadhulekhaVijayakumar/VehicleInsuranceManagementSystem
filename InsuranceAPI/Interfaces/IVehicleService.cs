using InsuranceAPI.Models.DTOs;

namespace InsuranceAPI.Interfaces
{
    public interface IVehicleService
    {
        Task<CreateVehicleResponse> RegisterVehicle(CreateVehicleRequest request);
        Task<IEnumerable<VehicleDto>> GetAllVehiclesByClient(int clientId);
        Task<VehicleDto> GetVehicleById(int vehicleId);
    }
}
