using Final.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Final.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentActivities()
        {
            var activities = await _activityService.GetRecentActivities(10);
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var activity = await _activityService.Get(id);
            if (activity == null)
            {
                return NotFound();
            }
            return Ok(activity);
        }

    }
}
