using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class CommentHistory : BaseEntity
    {
        public string PreviousContent { get; set; }
        public int CommentId { get; set; }
        public Comment Comment { get; set; } // Navigation property back to Comment
    }
}
