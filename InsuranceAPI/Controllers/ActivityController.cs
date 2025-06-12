using InsuranceAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityController(IActivityLogService activityLogService) 
        {
            _activityLogService = activityLogService;

        }
        [HttpGet("activities/recent")]
        public async Task<IActionResult> GetRecentActivities()
        {
            var activities = await _activityLogService.GetRecentActivitiesAsync();
            return Ok(activities);
        }

    }
}
