using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{
    public class CommentReactionRepository : Repository<CommentReaction>, ICommentReactionRepository
    {
        public CommentReactionRepository(FinalDbContext context) : base(context)
        {
        }
    }
}
