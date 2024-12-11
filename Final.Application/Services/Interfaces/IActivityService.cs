using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Implementations;

namespace Final.Application.Services
{
    public interface IActivityService
    {
        Task LogActivity(Activ activity);
        Task<List<Activ>> GetRecentActivities(int count);
        Task<Activ> Get(int id);
    }

    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ActivityService(IActivityRepository activityRepository, IUnitOfWork unitOfWork)
        {
            _activityRepository = activityRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task LogActivity(Activ activity)
        {
            activity.CreatedDate = DateTime.UtcNow;
            await _activityRepository.AddAsync(activity);
            _unitOfWork.Commit();
        }

        public async Task<List<Activ>> GetRecentActivities(int count)
        {
            return await _activityRepository.GetRecentActivities(count);
        }

        public async Task<Activ> Get(int id)
        {
            return await _activityRepository.Get(id);
        }
    }
}
