using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{
    public class CommentHistoryRepository : Repository<CommentHistory>, ICommentHistoryRepository
    {
        public CommentHistoryRepository(FinalDbContext context) : base(context)
        {
        }
    }
}
