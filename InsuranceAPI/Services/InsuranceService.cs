using InsuranceAPI.Context;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Misc;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InsuranceAPI.Services
{
    public class InsuranceService : IInsuranceService
    {
        private readonly InsuranceManagementContext _context;
        private readonly ILogger<InsuranceService> _logger;
        private readonly InsurancePolicyNumberGenerator _policyNumberGenerator;
        private readonly IRepository<int, Proposal> _proposalRepository;

        public InsuranceService(InsuranceManagementContext context, ILogger<InsuranceService> logger, InsurancePolicyNumberGenerator policyNumberGenerator, IRepository<int, Proposal> proposalRepository)
        {
            _context = context;
            _logger = logger;
            _policyNumberGenerator = policyNumberGenerator;
            _proposalRepository = proposalRepository;
        }


        public async Task<InsuranceQuoteResponse> GenerateQuote(int proposalId)
        {
            // Fetch the proposal
            var proposal = await _context.Proposals
                .FirstOrDefaultAsync(p => p.ProposalId == proposalId);

            if (proposal == null)
                throw new Exception("Proposal not found");

            if (proposal.Status != "approved")
                throw new Exception("Proposal is not approved");

            // Fetch the related insurance details
            var insuranceDetails = await _context.InsuranceDetails
                .FirstOrDefaultAsync(i => i.ProposalId == proposalId);

            if (insuranceDetails == null)
                throw new Exception("Insurance details not found");

            // Update the status in InsuranceDetails to "quote generated"
            proposal.Status = "quote generated";
            await _context.SaveChangesAsync();

            // Return the quote response
            return new InsuranceQuoteResponse
            {
                ProposalId = proposalId,
                PremiumAmount = insuranceDetails.CalculatedPremium,
                InsuranceSum = insuranceDetails.InsuranceSum,
                InsuranceStartDate = insuranceDetails.InsuranceStartDate,
                Status = proposal.Status
            };
        }


        public async Task<InsuranceResponse> GenerateInsuranceAsync(int proposalId)
        {
            var proposal = await _context.Proposals
                .FirstOrDefaultAsync(p => p.ProposalId == proposalId);

            if (proposal == null)
            {
                _logger.LogError("Proposal not found for ID {ProposalId}", proposalId);
                throw new Exception("Proposal not found");
            }

            if (proposal.Status != "payment successful")
                throw new Exception("Payment is not made by the client");
            // Fetch related insurance details
            var insuranceDetails = await _context.InsuranceDetails
                .FirstOrDefaultAsync(i => i.ProposalId == proposalId);

            if (insuranceDetails == null)
            {
                _logger.LogError("Insurance details not found for proposal ID {ProposalId}", proposalId);
                throw new Exception("Insurance details not found.");
            }

            // Generate Insurance Policy Number using stored procedure
            var policyNumber = await _policyNumberGenerator.GeneratePolicyNumber();

            // Create Insurance object
            var insurance = new Insurance
            {
                InsurancePolicyNumber = policyNumber,
                ProposalId = proposalId,
                VehicleId = proposal.VehicleId,
                ClientId = proposal.ClientId,
                PremiumAmount = insuranceDetails.CalculatedPremium,
                InsuranceStartDate = insuranceDetails.InsuranceStartDate,
                InsuranceSum = insuranceDetails.InsuranceSum,
                Status = "active",
                CreatedAt = DateTime.Now
            };

            _context.Insurances.Add(insurance);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Insurance created with Policy Number {PolicyNumber} for Proposal ID {ProposalId}", policyNumber, proposalId);
           

            // Return response
            return new InsuranceResponse
            {
                ProposalId = proposalId,
                VehicleId = proposal.VehicleId,
                ClientId = proposal.ClientId,
                InsurancePolicyNumber = insurance.InsurancePolicyNumber,
                PremiumAmount = insurance.PremiumAmount,
                InsuranceStartDate=insurance.InsuranceStartDate,
                InsuranceSum=insurance.InsuranceSum,
                Status = insurance.Status
            };
        }

        public async Task<InsuranceResponseDto> GetInsuranceByProposalIdForClientAsync(int proposalId, ClaimsPrincipal user)
        {
            var clientIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (clientIdClaim == null)
                throw new UnauthorizedAccessException("Client ID not found in token.");

            if (!int.TryParse(clientIdClaim.Value, out int clientId))
                throw new UnauthorizedAccessException("Invalid Client ID in token.");

            var insurance = await _context.Insurances
                .Include(i => i.Vehicle)
                .Include(i => i.Client)
                .Include(i => i.Proposal)
                .ThenInclude(p => p.InsuranceDetails)
                .FirstOrDefaultAsync(i => i.ProposalId == proposalId && i.ClientId == clientId);

            if (insurance == null)
                throw new Exception("Insurance not found.");

            var detail = insurance.Proposal?.InsuranceDetails;

            return new InsuranceResponseDto
            {
                InsurancePolicyNumber = insurance.InsurancePolicyNumber,
                VehicleNumber = insurance.Vehicle?.VehicleNumber,
                Name = insurance.Client?.Name,
                InsuranceType = insurance.Proposal?.InsuranceType,
                Plan = detail?.Plan,
                InsuranceStartDate = insurance.InsuranceStartDate,
                InsuranceSum = insurance.InsuranceSum,
                PremiumAmount = insurance.PremiumAmount,
                Status = insurance.Status,
                ExpiryDate = insurance.InsuranceStartDate.AddMonths(12)
            };
        }
        public async Task<List<InsuranceResponseDto>> GetAllInsurancesForClientAsync(ClaimsPrincipal user)
        {
            var clientIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (clientIdClaim == null)
                throw new UnauthorizedAccessException("Client ID not found in token.");

            if (!int.TryParse(clientIdClaim.Value, out int clientId))
                throw new UnauthorizedAccessException("Invalid Client ID in token.");

            var insurances = await _context.Insurances
                .Include(i => i.Vehicle)
                .Include(i => i.Client)
                .Include(i => i.Proposal)
                    .ThenInclude(p => p.InsuranceDetails)
                .Where(i => i.ClientId == clientId)
                .ToListAsync();

            return insurances.Select(insurance =>
            {
                var detail = insurance.Proposal?.InsuranceDetails;
                return new InsuranceResponseDto
                {
                    InsurancePolicyNumber = insurance.InsurancePolicyNumber,
                    VehicleNumber = insurance.Vehicle?.VehicleNumber,
                    Name = insurance.Client?.Name,
                    InsuranceType = insurance.Proposal?.InsuranceType,
                    Plan = detail?.Plan,
                    InsuranceStartDate = insurance.InsuranceStartDate,
                    InsuranceSum = insurance.InsuranceSum,
                    PremiumAmount = insurance.PremiumAmount,
                    Status = insurance.Status,
                    ExpiryDate = insurance.InsuranceStartDate.AddMonths(12)
                };
            }).ToList();
        }


        public async Task<PaginatedResult<InsuranceResponseDto>> GetAllInsurancesForAdminAsync(int page, int pageSize)
        {
            var query = _context.Insurances
                .Include(i => i.Vehicle)
                .Include(i => i.Client)
                .Include(i => i.Proposal)
                    .ThenInclude(p => p.InsuranceDetails)
                .AsQueryable();

            var totalRecords = await query.CountAsync();

            var insurances = await query
                .OrderByDescending(i => i.InsuranceStartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var results = insurances.Select(i =>
            {
                var detail = i.Proposal.InsuranceDetails;


                return new InsuranceResponseDto
                {
                    InsurancePolicyNumber = i.InsurancePolicyNumber,
                    VehicleNumber = i.Vehicle?.VehicleNumber,
                    Name = i.Client?.Name,
                    InsuranceType = i.Proposal?.InsuranceType,
                    Plan = detail?.Plan,
                    InsuranceStartDate = i.InsuranceStartDate,
                    InsuranceSum = i.InsuranceSum,
                    PremiumAmount = i.PremiumAmount,
                    Status = i.Status,
                    ExpiryDate = i.InsuranceStartDate.AddMonths(12)
                };
            }).ToList();

            return new PaginatedResult<InsuranceResponseDto>
            {
                Data = results,
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize
            };
        }



        public async Task<IEnumerable<ClientPolicyStatusDto>> GetClientPolicyStatusAsync(int clientId)
        {
            var proposals = await _context.Proposals
                .Include(p => p.Vehicle)
                .Include(p => p.InsuranceDetails)
                .Include(p => p.Insurance)
                .Where(p => p.ClientId == clientId && p.Insurance == null)
                .ToListAsync();


            var result = proposals.Select(p => new ClientPolicyStatusDto
            {
                ProposalId = p.ProposalId,
                VehicleNumber = p.Vehicle?.VehicleNumber,
                VehicleType = p.Vehicle?.VehicleType,
                InsuranceType = p.InsuranceType,
                ProposalCreatedAt = p.CreatedAt,
                ProposalStatus = p.Status,

                CalculatedPremium = p.InsuranceDetails?.CalculatedPremium,
                InsuranceStartDate = p.InsuranceDetails?.InsuranceStartDate,
                InsuranceSum = p.InsuranceDetails?.InsuranceSum,

   
            });

            return result;
        }

        public async Task<Insurance> GetInsuranceWithDetailsAsync(string policyNumber)
        {
            return await _context.Insurances
                .Include(i => i.Client)
                .Include(i => i.Vehicle)
                .Include(i => i.Proposal)
                
                .FirstOrDefaultAsync(i => i.InsurancePolicyNumber == policyNumber);
        }

        public async Task<IEnumerable<ProposalReviewDto>> GetPaidProposals()
        {
            var proposals = await _proposalRepository.GetAll();

            var result = proposals
                .Where(p => p.Status == "payment successful" && p.Insurance == null)
                .Select(p => new ProposalReviewDto
                {
                    ClientId = p.ClientId,
                    ClientName = p.Client?.Name ?? "Unknown",
                    ProposalId = p.ProposalId,
                    InsuranceType = p.InsuranceType,
                    Status = p.Status
                });

            return result;
        }
    }


}
