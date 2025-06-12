using InsuranceAPI.Context;
using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Repositories
{
    public class InsuranceDetailsRepository:Repository<int, InsuranceDetails>   
    {
        public InsuranceDetailsRepository(InsuranceManagementContext context ): base(context){ }
        public override async Task<InsuranceDetails> GetById(int key)
        {
            var insuranceDetails = await _context.InsuranceDetails
                .Include(e => e.Proposal)
                .Include(e => e.Vehicle)
                .SingleOrDefaultAsync(e => e.Id == key);

            if (insuranceDetails == null)
                throw new Exception($"Insurance details with ID {key} not present");

            return insuranceDetails;
        }
        public override async Task<IEnumerable<InsuranceDetails>> GetAll()
        {
            var insuranceDetails = _context.InsuranceDetails
                .Include(e => e.Proposal)
                .Include(e => e.Vehicle);

            if (insuranceDetails.Count() == 0)
                throw new Exception("No insurance details found");

            return insuranceDetails;
        }
    }
}
