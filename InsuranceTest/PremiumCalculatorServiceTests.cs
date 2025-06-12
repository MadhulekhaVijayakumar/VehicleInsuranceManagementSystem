using InsuranceAPI.Models;
using InsuranceAPI.Services;
using NUnit.Framework;

namespace InsuranceTest
{
    [TestFixture]
    public class PremiumCalculatorServiceTests
    {
        private PremiumCalculatorService _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new PremiumCalculatorService();
        }

       

        [Test]
        public void CalculatePremium_MinimumPremium_ReturnsAtLeast2000()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                VehicleType = "Bike",
                FuelType = "Petrol",
                SeatCapacity = 2
            };

            var details = new InsuranceDetails
            {
                InsuranceSum = 100,
                Plan = "Silver",
                DamageInsurance = "None",
                LiabilityOption = "None"
            };

            // Act
            var result = _calculator.CalculatePremium(details, vehicle);

            // Assert
            Assert.GreaterOrEqual(result, 2000);
        }
    }
}