using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{
    public class UserCardRepository : Repository<UserCard>, IUserCardRepository
    {
        public UserCardRepository(FinalDbContext context) : base(context)
        {
        }
    }
}
