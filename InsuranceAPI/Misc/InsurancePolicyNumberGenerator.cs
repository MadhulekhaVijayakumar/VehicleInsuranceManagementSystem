using InsuranceAPI.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Misc
{
    public class InsurancePolicyNumberGenerator
    {
        private readonly InsuranceManagementContext _context;

        public InsurancePolicyNumberGenerator(InsuranceManagementContext context)
        {
            _context = context;
        }

        public async Task<string> GeneratePolicyNumber()
        {
            var outputParam = new SqlParameter
            {
                ParameterName = "@InsurancePolicyNumber",
                SqlDbType = System.Data.SqlDbType.VarChar,
                Size = 10,
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC proc_GenerateInsurancePolicyNumber @InsurancePolicyNumber OUT", outputParam);
            return outputParam.Value.ToString();
        }
    }
}
