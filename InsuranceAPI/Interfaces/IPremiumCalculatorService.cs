using InsuranceAPI.Models;

namespace InsuranceAPI.Interfaces
{
    public interface IPremiumCalculatorService
    {
        decimal CalculatePremium(InsuranceDetails details, Vehicle vehicle);
    }
}
