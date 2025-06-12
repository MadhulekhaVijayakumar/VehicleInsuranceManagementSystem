using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;
using InsuranceAPI.Context;

namespace InsuranceAPI.Repositories
{
    public class PaymentRepository:Repository<int,Payment>
    {
        public PaymentRepository(InsuranceManagementContext context) : base(context)
        {
        }
        public override async Task <Payment> GetById(int key)
        {
            var payment = await _context.Payments
                .SingleOrDefaultAsync(e => e.PaymentId == key);

            if (payment == null)
                throw new Exception($"Payment with ID {key} not present");

            return payment;
        }
        public override async Task<IEnumerable<Payment>> GetAll()
        {
            var payments = await _context.Payments.ToListAsync(); 

            if (!payments.Any()) 
                throw new Exception("No payments found");

            return payments;
        }

    }
}
