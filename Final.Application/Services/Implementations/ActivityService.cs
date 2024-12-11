//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Final.Application.Services.Implementations
//{
//    public class ActivityService : IActivityService
//    {
//        private readonly IActivityRepository _activityRepository;

//        public ActivityService(IActivityRepository activityRepository)
//        {
//            _activityRepository = activityRepository;
//        }

//        public async Task LogActivity(Activity activity)
//        {
//            activity.Timestamp = DateTime.UtcNow;
//            await _activityRepository.AddAsync(activity);
//        }

//        public async Task<List<Activity>> GetRecentActivities(int count)
//        {
//            return await _activityRepository.GetRecentActivities(count);
//        }
//    }
//}
