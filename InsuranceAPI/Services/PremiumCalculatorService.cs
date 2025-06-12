using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;

namespace InsuranceAPI.Services
{
    public class PremiumCalculatorService : IPremiumCalculatorService
    {
        public decimal CalculatePremium(InsuranceDetails details, Vehicle vehicle)
        {
            decimal baseRate = 0.03m; // 3% of insurance sum
            decimal premium = details.InsuranceSum * baseRate;

            // Adjust premium based on vehicle type
            switch (vehicle.VehicleType.ToLower())
            {
                case "car":
                    premium += 1000;
                    break;
                case "bike":
                    premium += 500;
                    break;
                case "camper van":
                    premium += 1500;
                    break;
            }

            // Adjust premium based on fuel type
            if (vehicle.FuelType.ToLower() == "diesel")
            {
                premium += 1000;
            }

            // Adjust based on seat capacity
            if (vehicle.SeatCapacity > 5)
            {
                premium += 500;
            }

            // Plan adjustment
            switch (details.Plan.ToLower())
            {
                case "silver":
                    premium += 500;
                    break;
                case "gold":
                    premium += 1000;
                    break;
                case "platinum":
                    premium += 1500;
                    break;
            }

            // Damage Insurance
            switch (details.DamageInsurance.ToLower())
            {
                case "false":
                    premium += 750;
                    break;
                case "true":
                    premium += 1500;
                    break;
            }

            // Liability Option
            if (details.LiabilityOption.ToLower() == "own damage")
            {
                premium += 1000;
            }

            // Clamp premium to at least 2000
            return Math.Max(premium, 2000);
        }
    }
}
