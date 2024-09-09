using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{
    public class BasketRepository : Repository<Basket>, IBasketRepository
    {
        public BasketRepository(FinalDbContext context) : base(context)
        {
        }
    }

}
