using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{
    public class GamePlatformRepository : Repository<GamePlatform>, IGamePlatformRepository
    {
        public GamePlatformRepository(FinalDbContext context) : base(context)
        {
        }
    }
}
