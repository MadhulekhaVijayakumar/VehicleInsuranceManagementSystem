using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Repositories;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace InsuranceAPI.Services
{
    public class ProposalService : IProposalService
    {
        private readonly IRepository<int, Proposal> _proposalRepository;
        private readonly IRepository<int, Vehicle> _vehicleRepository;
        private readonly IRepository<int, InsuranceDetails> _insuranceDetailsRepository;
        private readonly IPremiumCalculatorService _premiumCalculatorService;
        private readonly ILogger<ProposalService> _logger;
        private readonly IDocumentService _documentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActivityLogService _activityLogService;
        private readonly IRepository<int, Document> _documentRepository;



        public ProposalService(
            IRepository<int, Proposal> proposalRepo,
            IRepository<int, Vehicle> vehicleRepo,
            IRepository<int, InsuranceDetails> insuranceRepo,
            IRepository<int,Document> documentRepository,
            IPremiumCalculatorService premiumCalculatorService,
            IDocumentService documentService,
            ILogger<ProposalService> logger,
            IHttpContextAccessor httpContextAccessor,
            IActivityLogService activityLogService)
        {
            _proposalRepository = proposalRepo;
            _vehicleRepository = vehicleRepo;
            _insuranceDetailsRepository = insuranceRepo;
            _documentRepository = documentRepository;
            _premiumCalculatorService = premiumCalculatorService;
            _documentService = documentService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _activityLogService = activityLogService;
        }

        public async Task<CreateProposalResponse> SubmitProposalWithDetails(CreateProposalRequest request)
        {
            _logger.LogInformation("Starting proposal submission process.");
            try
            {

                var clientIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out int clientId))
                {
                    _logger.LogWarning("Client ID missing or invalid in token.");
                    throw new UnauthorizedAccessException("Client ID not found in token.");
                }

                _logger.LogInformation("Creating vehicle for ClientId: {ClientId}", clientId);
                var vehicle = new Vehicle
                {
                    ClientId = clientId,
                    VehicleType = request.Vehicle.VehicleType,
                    VehicleNumber = request.Vehicle.VehicleNumber,
                    ChassisNumber = request.Vehicle.ChassisNumber,
                    EngineNumber = request.Vehicle.EngineNumber,
                    MakerName = request.Vehicle.MakerName,
                    ModelName = request.Vehicle.ModelName,
                    VehicleColor = request.Vehicle.VehicleColor,
                    FuelType = request.Vehicle.FuelType,
                    RegistrationDate = request.Vehicle.RegistrationDate,
                    SeatCapacity = request.Vehicle.SeatCapacity
                };

                var createdVehicle = await _vehicleRepository.Add(vehicle);
                _logger.LogInformation("Vehicle created with ID: {VehicleId}", createdVehicle.VehicleId);

                var proposal = new Proposal
                {
                    ClientId = clientId,
                    VehicleId = createdVehicle.VehicleId,
                    InsuranceType = request.Proposal.InsuranceType,
                    InsuranceValidUpto = request.Proposal.InsuranceValidUpto,
                    FitnessValidUpto = request.Proposal.FitnessValidUpto,
                    Status = "submitted",
                    CreatedAt = DateTime.Now
                };

                var createdProposal = await _proposalRepository.Add(proposal);
                _logger.LogInformation("Proposal created with ID: {ProposalId}", createdProposal.ProposalId);

                var insuranceDetails = new InsuranceDetails
                {
                    ProposalId = createdProposal.ProposalId,
                    VehicleId = createdVehicle.VehicleId,
                    InsuranceStartDate = request.InsuranceDetails.InsuranceStartDate,
                    InsuranceSum = request.InsuranceDetails.InsuranceSum,
                    DamageInsurance = request.InsuranceDetails.DamageInsurance,
                    LiabilityOption = request.InsuranceDetails.LiabilityOption,
                    Plan = request.InsuranceDetails.Plan
                };

                var premium = _premiumCalculatorService.CalculatePremium(insuranceDetails, vehicle);
                insuranceDetails.CalculatedPremium = premium;

                _logger.LogInformation("Calculated premium: {Premium}", premium);
                await _insuranceDetailsRepository.Add(insuranceDetails);
                _logger.LogInformation("Insurance details saved for ProposalId: {ProposalId}", createdProposal.ProposalId);

                if (request.Documents != null)
                {
                    try
                    {
                        request.Documents.ProposalId = createdProposal.ProposalId;
                        await _documentService.SaveDocumentsAsync(request.Documents);
                        _logger.LogInformation("Documents saved successfully for ProposalId: {ProposalId}", createdProposal.ProposalId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Document upload failed for ProposalId: {ProposalId}", createdProposal.ProposalId);
                        throw; // Optional: rethrow if you want to stop the process
                    }
                }

                _logger.LogInformation("Proposal submission completed for ProposalId: {ProposalId}", createdProposal.ProposalId);

                return new CreateProposalResponse
                {
                    ProposalId = createdProposal.ProposalId,
                    Status = createdProposal.Status,
                    CalculatedPremium = premium
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during proposal submission");
                throw; // Optional: rethrow if you want to stop the process
            }
        }



         public async Task<IEnumerable<ProposalReviewDto>> GetSubmittedProposals()
         {
            var proposals = await _proposalRepository.GetAll();

            var result = proposals
                .Where(p => p.Status == "submitted")
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
        public async Task<ProposalDetailsResponse> GetProposalDetailsByProposalIdAsync(int proposalId)
        {
            var proposal = await _proposalRepository.GetById(proposalId);
            if (proposal == null) return null;

            var vehicle = await _vehicleRepository.GetById(proposal.VehicleId);
            var insuranceDetails = proposal.InsuranceDetails;

            var documents = await _documentRepository.GetAll();
            var filteredDocs = documents.Where(d => d.ProposalId == proposalId).ToList();

            string GetFileName(string type) =>
                filteredDocs.FirstOrDefault(d => d.FileType.ToLower() == type.ToLower())?.FileName;

            return new ProposalDetailsResponse
            {
                Vehicle = new CreateVehicleRequest
                {
                    VehicleType = vehicle?.VehicleType ?? "",
                    VehicleNumber = vehicle?.VehicleNumber ?? "",
                    ChassisNumber = vehicle?.ChassisNumber ?? "",
                    EngineNumber = vehicle?.EngineNumber ?? "",
                    MakerName = vehicle?.MakerName ?? "",
                    ModelName = vehicle?.ModelName ?? "",
                    VehicleColor = vehicle?.VehicleColor ?? "",
                    FuelType = vehicle?.FuelType ?? "",
                    RegistrationDate = vehicle?.RegistrationDate ?? DateTime.MinValue,
                    SeatCapacity = vehicle?.SeatCapacity ?? 0
                },
                Proposal = new CreateProposalData
                {
                    InsuranceType = proposal.InsuranceType,
                    InsuranceValidUpto = proposal.InsuranceValidUpto,
                    FitnessValidUpto = proposal.FitnessValidUpto
                },
                InsuranceDetails = new CreateInsuranceDetailRequest
                {
                    InsuranceStartDate = insuranceDetails?.InsuranceStartDate ?? DateTime.MinValue,
                    InsuranceSum = insuranceDetails?.InsuranceSum ?? 0,
                    DamageInsurance = insuranceDetails?.DamageInsurance ?? "",
                    LiabilityOption = insuranceDetails?.LiabilityOption ?? "",
                    Plan = insuranceDetails?.Plan ?? ""
                },
                Documents = new ProposalDocumentFileNames
                {
                    LicenseFileName = GetFileName("License"),
                    RCBookFileName = GetFileName("RC Book"),
                    PollutionCertificateFileName = GetFileName("Pollution Certificate")
                },
                ClientId = proposal.ClientId
            };
        }

        public async Task<bool> VerifyProposal(int proposalId, bool approve)
        {
            var proposal = await _proposalRepository.GetById(proposalId);
            if (proposal == null || proposal.Status != "submitted")
                return false;

            proposal.Status = approve ? "approved" : "rejected";
            await _proposalRepository.Update(proposalId, proposal);
            var adminName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown Admin";
            await _activityLogService.LogActivityAsync(adminName, $"Verified proposal #{proposalId}: {(approve ? "approved" : "rejected")}");


            return true;
        }

        public async Task<PagedProposalResponse> GetProposalsPagedAsync(ProposalQueryParameters queryParameters)
        {
            var allProposals = await _proposalRepository.GetAll();

            // Apply Search (optional)
            if (!string.IsNullOrWhiteSpace(queryParameters.Search))
            {
                string search = queryParameters.Search.ToLower();
                allProposals = allProposals.Where(p =>
                    p.Vehicle.VehicleNumber.ToLower().Contains(search) ||
                    p.Client.Name.ToLower().Contains(search));
            }

            // Apply Status Filter (optional)
            if (!string.IsNullOrWhiteSpace(queryParameters.Status))
            {
                allProposals = allProposals.Where(p => p.Status.ToLower() == queryParameters.Status.ToLower());
            }

            var totalCount = allProposals.Count();

            var pagedProposals = allProposals
                .OrderByDescending(p => p.CreatedAt)
                .Skip((queryParameters.Page - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .Select(p => new ProposalDto
                {
                    ProposalId = p.ProposalId,
                    ClientName = p.Client?.Name ?? "Unknown", // Safely check if Client is null
                    VehicleNumber = p.Vehicle?.VehicleNumber ?? "Unknown", // Safely check if Vehicle is null
                    Status = p.Status,
                    InsuranceType = p.InsuranceType ?? "Not Available", // Access directly from Proposal table
                    InsuranceValidUpto = p.InsuranceValidUpto ?? DateTime.MinValue, // Access directly from Proposal table
                    FitnessValidUpto = p.FitnessValidUpto, // Access directly from Proposal table
                    CreatedAt = p.CreatedAt
                })
                .ToList();

            return new PagedProposalResponse
            {
                Proposals = pagedProposals,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<ProposalReviewDto>> GetApprovedProposals()
        {
            var proposals = await _proposalRepository.GetAll();

            var result = proposals
                .Where(p => p.Status == "approved")
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

        public async Task<IEnumerable<ProposalReviewDto>> GetQuotesProposals(int clientId)
        {
            var proposals = await _proposalRepository.GetAll();

            var result = proposals
                .Where(p => p.Status == "quote generated" && p.ClientId == clientId)
                .Select(p => new ProposalReviewDto
                {
                    ClientId = p.ClientId,
                    ClientName = p.Client?.Name ?? "Unknown",
                    ProposalId = p.ProposalId,
                    InsuranceType = p.InsuranceType,
                    Status = p.Status,

                    InsuranceSum = (decimal)(p.InsuranceDetails?.InsuranceSum),
                    CalculatedPremium = (decimal)(p.InsuranceDetails?.CalculatedPremium),
                    InsuranceStartDate = p.InsuranceDetails?.InsuranceStartDate
                });

            return result;
        }

        



    }
}
