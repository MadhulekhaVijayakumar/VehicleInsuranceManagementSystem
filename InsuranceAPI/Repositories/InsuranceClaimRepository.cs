using InsuranceAPI.Context;
using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Repositories
{
    public class InsuranceClaimRepository : Repository<int, InsuranceClaim>
    {
        public InsuranceClaimRepository(InsuranceManagementContext context) : base(context) { }

        public override async Task<InsuranceClaim> GetById(int key)
        {
            var claim = await _context.InsuranceClaims
                .Include(c => c.Insurance)
                .FirstOrDefaultAsync(c => c.ClaimId == key);

            if (claim == null)
                throw new Exception($"Claim with ID {key} not found");

            return claim;
        }

        public override async Task<IEnumerable<InsuranceClaim>> GetAll()
        {
            var claims = await _context.InsuranceClaims
                .Include(c => c.Insurance)
                .ToListAsync();

            if (!claims.Any())
                throw new Exception("No claims found");

            return claims;
        }

       
    }
}
