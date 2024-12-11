namespace Final.Mvc.ViewModels.CommentVMs
{
    public class CommentReplyVM
    {
        public int ParentCommentId { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
    }
}
