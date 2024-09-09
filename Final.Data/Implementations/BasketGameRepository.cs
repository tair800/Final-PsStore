using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{
    public class BasketGameRepository : Repository<BasketGame>, IBasketGameRepository
    {
        public BasketGameRepository(FinalDbContext context) : base(context)
        {
        }
    }

}
