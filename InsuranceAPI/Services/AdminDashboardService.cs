using InsuranceAPI.Context;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly InsuranceManagementContext _context;

        public AdminDashboardService(InsuranceManagementContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var totalClients = await _context.Clients.CountAsync();

            var pendingProposals = await _context.Proposals
                .CountAsync(p => p.Status == "submitted");

            var claimsToReview = await _context.InsuranceClaims
                .CountAsync(c => c.Status == "pending");

            var totalRevenue = await _context.Payments
                .SumAsync(p => (decimal?)p.AmountPaid) ?? 0;

            return new AdminDashboardSummaryDto
            {
                TotalClients = totalClients,
                PendingProposals = pendingProposals,
                ClaimsToReview = claimsToReview,
                TotalRevenue = totalRevenue
            };
        }
    }

}
