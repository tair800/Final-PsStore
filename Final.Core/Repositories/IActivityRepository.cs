using Final.Core.Entities;

namespace Final.Core.Repositories
{
    public interface IActivityRepository
    {
        Task AddAsync(Activ activity);
        Task<List<Activ>> GetRecentActivities(int count);
        Task<Activ> Get(int id);

    }
}
