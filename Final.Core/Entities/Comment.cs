using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; }
        public string UserId { get; set; }

        public int GameId { get; set; }
        public User User { get; set; }
        public Game Game { get; set; }

        public ICollection<CommentReaction> CommentReactions { get; set; } = new List<CommentReaction>();

        public ICollection<CommentHistory> CommentHistories { get; set; }


    }
}
