using InsuranceAPI.Context;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly InsuranceManagementContext _context;

        public ActivityLogService(InsuranceManagementContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(string username, string action)
        {
            var log = new ActivityLog
            {
                Username = username,
                Action = action,
                Timestamp = DateTime.UtcNow
            };
            _context.ActivityLog.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ActivityLog>> GetRecentActivitiesAsync(int count = 10)
        {
            return await _context.ActivityLog
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToListAsync();
        }
    }

}
