using InsuranceAPI.Context;
using InsuranceAPI.Exceptions;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InsuranceAPI.Services
{
    public class InsuranceClaimService : IInsuranceClaimService
    {
        private readonly IRepository<int, InsuranceClaim> _claimRepository;
        private readonly IRepository<string, Insurance> _insuranceRepository;
        private readonly IDocumentService _documentService;
        private readonly IRepository<int, Document> _documentRepository;
        private readonly InsuranceManagementContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActivityLogService _activityLogService;

        public InsuranceClaimService(
            IRepository<int, InsuranceClaim> claimRepository,
            IRepository<string, Insurance> insuranceRepository,
            IRepository<int, Document> documentRepository,
            IDocumentService documentService, InsuranceManagementContext context,
            IHttpContextAccessor httpContextAccessor,
            IActivityLogService activityLogService)
            
        {
            _claimRepository = claimRepository;
            _insuranceRepository = insuranceRepository;
            _documentService = documentService;
            _documentRepository = documentRepository;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _activityLogService = activityLogService;
        }


        public async Task<CreateClaimResponse> SubmitClaimWithDocumentsAsync(CreateClaimRequest request, ClaimsPrincipal user)
        {
            try
            {
                var clientId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(clientId))
                    throw new Exception("Invalid client information");

                var insurance = await _insuranceRepository.GetById(request.InsurancePolicyNumber);
                if (insurance == null || insurance.ClientId.ToString() != clientId)
                    throw new Exception("Invalid insurance policy for this client");

                var claim = new InsuranceClaim
                {
                    InsurancePolicyNumber = request.InsurancePolicyNumber,
                    IncidentDate = request.IncidentDate,
                    Description = request.Description,
                    ClaimAmount = request.ClaimAmount,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending"
                };

                var savedClaim = await _claimRepository.Add(claim);

                if (request.Documents != null)
                {
                    await _documentService.SaveClaimDocumentsAsync(savedClaim.ClaimId, request.Documents);
                }


                return new CreateClaimResponse
                {
                    ClaimId = savedClaim.ClaimId,
                    Status = savedClaim.Status,
                    Message = "Claim submitted successfully"
                };
            }
            catch (Exception ex)
            {
                return new CreateClaimResponse
                {
                    ClaimId = 0,
                    Status = "Pending",
                    Message = $"Error: {ex.Message} --- INNER: {ex.InnerException?.Message}"
                };
            }

        }

        public async Task<IEnumerable<InsuranceClaimDto>> GetClaimsByClientAsync(ClaimsPrincipal user)
        {
            var clientId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId))
                throw new Exception("Invalid client");

            var allClaims = await _claimRepository.GetAll(); // Assume this fetches all claims

            var userClaims = allClaims
                .Where(c => c.Insurance != null && c.Insurance.ClientId.ToString() == clientId)
                .Select(c => new InsuranceClaimDto
                {
                    ClaimId = c.ClaimId,
                    InsurancePolicyNumber = c.InsurancePolicyNumber,
                    IncidentDate = c.IncidentDate,
                    ClaimAmount = c.ClaimAmount,
                    Status = c.Status
                   
                });

            return userClaims;
        }

        public async Task<List<PendingClaimDto>> GetAllPendingClaimsAsync()
        {
            var pendingClaims = await _context.InsuranceClaims
                .Where(c => c.Status == "Pending")
                .Select(c => new PendingClaimDto
                {
                    ClaimId = c.ClaimId,
                    InsurancePolicyNumber = c.InsurancePolicyNumber,
                    ClaimAmount = c.ClaimAmount ?? 0m, // Provide a default value of 0 if it's null

                    ClaimStatus = c.Status
                })
                .ToListAsync();

            if (pendingClaims == null || !pendingClaims.Any())
                throw new NotFoundException("No pending claims found.");

            return pendingClaims;
        }
        public async Task<List<PendingClaimDto>> GetAllApprovedClaimsAsync()
        {
            var pendingClaims = await _context.InsuranceClaims
                .Where(c => c.Status == "Approved")
                .Select(c => new PendingClaimDto
                {
                    ClaimId = c.ClaimId,
                    InsurancePolicyNumber = c.InsurancePolicyNumber,
                    ClaimAmount = c.ClaimAmount ?? 0m, // Provide a default value of 0 if it's null

                    ClaimStatus = c.Status
                })
                .ToListAsync();

            if (pendingClaims == null || !pendingClaims.Any())
                return new List<PendingClaimDto>();

            return pendingClaims;
        }
        public async Task<int> SetApprovedClaimsToSettledAsync()
        {
            var adminName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
            var approvedClaims = await _context.InsuranceClaims
                .Where(c => c.Status == "Approved")
                .ToListAsync();

            if (!approvedClaims.Any())
                throw new NotFoundException("No approved claims found to settle.");

            int updatedCount = 0;
            foreach (var claim in approvedClaims)
            {
                if (claim.Status != "Claim Settled")  // Only update if status is not already "Claim Settled"
                {
                    claim.Status = "Claim Settled";
                    await _activityLogService.LogActivityAsync(adminName, $"Settled claim #{claim.ClaimId} (status changed to 'Claim Settled')");
                    updatedCount++;
                }
            }

            // Save changes and return the count of updated claims
            await _context.SaveChangesAsync();

            return updatedCount > 0 ? updatedCount : throw new Exception("No claims were updated.");
        }



        public async Task<ClaimReviewDto> GetClaimDetailsForReviewAsync(int claimId)
        {
            var claim = await _context.InsuranceClaims
                .Include(c => c.Insurance)
                    .ThenInclude(i => i.Client)
                .Include(c => c.Insurance.Vehicle)
                .FirstOrDefaultAsync(c => c.ClaimId == claimId);

            if (claim == null)
                throw new NotFoundException($"Claim with ID {claimId} not found.");
            var documents = await _documentRepository.GetAll();
            var filteredDocs = documents.Where(d => d.ClaimId == claimId).ToList();

            string GetFileName(string type) =>
                filteredDocs.FirstOrDefault(d => d.FileType.ToLower() == type.ToLower())?.FileName;
            if (filteredDocs == null || !filteredDocs.Any())
                throw new NotFoundException($"No documents found for claim ID {claimId}.");

            var insurance = claim.Insurance;
            var client = insurance.Client;
            var vehicle = insurance.Vehicle;

            return new ClaimReviewDto
            {
                ClaimId = claim.ClaimId,
                IncidentDate = claim.IncidentDate,
                ClaimAmount = claim.ClaimAmount ?? 0m,
                Description = claim.Description,
                Status = claim.Status,
               
               

                InsurancePolicyNumber = insurance.InsurancePolicyNumber,
                InsuranceStartDate = insurance.InsuranceStartDate,
                InsuranceSum = insurance.InsuranceSum,
                PremiumAmount = insurance.PremiumAmount,
                InsuranceStatus = insurance.Status,

                ClientName = client?.Name,
                Email = client?.Email,
                PhoneNumber = client?.PhoneNumber,
                Address = client?.Address,

                VehicleNumber = vehicle?.VehicleNumber,
                VehicleType = vehicle?.VehicleType,
                Make = vehicle?.MakerName,
                Model = vehicle?.ModelName,
                RegistrationDate = (DateTime)(vehicle?.RegistrationDate),

                Documents = new ClaimDocumentFileNames
                {
                    RepairEstimateCost = GetFileName("RepairEstimateCost"),
                    AccidentCopy = GetFileName("AccidentCopy"),
                    FIRCopy = GetFileName("FIRCopy")
                }
            };
        }



        public async Task<PaginatedResult<InsuranceClaimDto>> GetAllClaimsForAdminAsync(int pageNumber, int pageSize)
        {
            var allClaims = await _claimRepository.GetAll(); // Consider returning IQueryable for better performance
            var sortedClaims = allClaims
                .OrderByDescending(c => c.IncidentDate) // Sort by most recent incident
                .ToList();

            var totalRecords = sortedClaims.Count;

            var paginatedClaims = sortedClaims
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new InsuranceClaimDto
                {
                    ClaimId = c.ClaimId,
                    InsurancePolicyNumber = c.InsurancePolicyNumber,
                    IncidentDate = c.IncidentDate,
                    Description = c.Description,
                    ClaimAmount = c.ClaimAmount,
                    Status = c.Status
                })
                .ToList();

            return new PaginatedResult<InsuranceClaimDto>
            {
                Data = paginatedClaims,
                TotalRecords = totalRecords,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }




        public async Task<InsuranceClaim> UpdateClaimStatusAsync(int claimId, string newStatus)
        {
            var adminName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";

            var claim = await _claimRepository.GetById(claimId);
            if (claim == null)
                throw new Exception("Claim not found");

            if (newStatus != "Approved" && newStatus != "Rejected")
                throw new Exception("Invalid status value. Must be 'Approved' or 'Rejected'.");

            claim.Status = newStatus;

            await _activityLogService.LogActivityAsync(
       adminName,
       $"Updated claim #{claimId} status to {newStatus}");

            return await _claimRepository.Update(claimId, claim);
        }
    }
}
