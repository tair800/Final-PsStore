using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{
    public class DlcRepository : Repository<Dlc>, IDlcRepository
    {
        public DlcRepository(FinalDbContext context) : base(context)
        {

        }
    }

}
