using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;

namespace InsuranceAPI.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly IQuotePdfGenerator _quotePdfGenerator;
        private readonly IRepository<int, Client> _clientRepo;
        private readonly IRepository<int, Vehicle> _vehicleRepo;
        private readonly IRepository<int, InsuranceDetails> _insuranceRepo;
        private readonly IRepository<int, Proposal> _proposalRepo;

        public QuoteService(
            IQuotePdfGenerator quotePdfGenerator,
            IRepository<int,Client> clientRepo,
            IRepository<int, Proposal> proposalRepo,
            IRepository<int, Vehicle> vehicleRepo,
            IRepository<int, InsuranceDetails> insuranceRepo
            )
        {
            _quotePdfGenerator = quotePdfGenerator;
            _clientRepo = clientRepo;
            _vehicleRepo = vehicleRepo;
            _insuranceRepo = insuranceRepo;
            _proposalRepo = proposalRepo;
        }

        public async Task<byte[]> GenerateQuotePdfAsync(int proposalId)
        {
            var proposal = await _proposalRepo.GetById(proposalId);
            if (proposal == null) throw new Exception("Proposal not found.");

            var client = await _clientRepo.GetById(proposal.ClientId);
            var vehicle = await _vehicleRepo.GetById(proposal.VehicleId);
            var insurance = await _insuranceRepo.GetById(proposalId);

            if (client == null || vehicle == null || insurance == null)
                throw new Exception("Required data missing.");

            string quoteNumber = $"QUOTE-{proposalId:D5}";

            return _quotePdfGenerator.GenerateQuotePdf(quoteNumber, client, vehicle, proposal, insurance);
        }
    }

}
