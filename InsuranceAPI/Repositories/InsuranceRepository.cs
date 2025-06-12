using InsuranceAPI.Context;
using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Repositories
{
    public class InsuranceRepository: Repository<string, Insurance>
    {
        public InsuranceRepository(InsuranceManagementContext context) : base(context) { }

        public override async Task<Insurance> GetById(string key)
        {
            var insurance = await _context.Insurances
                .SingleOrDefaultAsync(e => e.InsurancePolicyNumber == key);

            if (insurance == null)
                throw new Exception($"Insurance with ID {key} not present");

            return insurance;
        }

        public override async Task<IEnumerable<Insurance>> GetAll()
        {
            var insurances = _context.Insurances.Include(e => e.Client);
            if (insurances.Count() == 0)
                throw new Exception("No insurances found");

            return insurances;
        }

    }
}
