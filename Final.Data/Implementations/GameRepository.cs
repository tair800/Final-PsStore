using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{
    public class GameRepository : Repository<Game>, IGameRepository
    {
        public GameRepository(FinalDbContext context) : base(context)
        {
        }
    }
}
