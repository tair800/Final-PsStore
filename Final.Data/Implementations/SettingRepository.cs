using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{
    public class SettingRepository : Repository<Setting>, ISettingRepository
    {
        public SettingRepository(FinalDbContext context) : base(context)
        {
        }
    }
}