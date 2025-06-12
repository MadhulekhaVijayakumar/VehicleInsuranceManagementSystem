using InsuranceAPI.Context;
using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Repositories
{
    public class VehicleRepository : Repository<int, Vehicle>
    {
        public VehicleRepository(InsuranceManagementContext context) : base(context) { }

        public override async Task<Vehicle> GetById(int key)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.Client)
                .FirstOrDefaultAsync(v => v.VehicleId == key);

            if (vehicle == null)
                throw new Exception($"Vehicle with ID {key} not found");

            return vehicle;
        }

        public override async Task<IEnumerable<Vehicle>> GetAll()
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.Client)
                .ToListAsync();

            if (vehicles.Count == 0)
                throw new Exception("No vehicles found");

            return vehicles;
        }
    }
}
