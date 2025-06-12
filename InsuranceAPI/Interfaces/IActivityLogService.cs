using InsuranceAPI.Models;

namespace InsuranceAPI.Interfaces
{
    public interface IActivityLogService
    {
        Task LogActivityAsync(string username, string action);
        Task<List<ActivityLog>> GetRecentActivitiesAsync(int count = 10);
    }

}
