using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{
    public class PromoRepository : Repository<Promo>, IPromoRepository
    {
        public PromoRepository(FinalDbContext context) : base(context)
        {
        }
    }
}
