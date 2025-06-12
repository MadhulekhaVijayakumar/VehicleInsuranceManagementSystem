using InsuranceAPI.Models.DTOs;

namespace InsuranceAPI.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardSummaryDto> GetDashboardSummaryAsync();
    }
}
