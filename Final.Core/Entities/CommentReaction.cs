using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class CommentReaction : BaseEntity
    {
        public int CommentId { get; set; }
        public string UserId { get; set; }
        public bool IsLike { get; set; }

        public Comment Comment { get; set; }
        public User User { get; set; }
    }
}
