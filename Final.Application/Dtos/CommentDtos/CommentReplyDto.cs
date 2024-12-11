namespace Final.Application.Dtos.CommentDtos
{
    public class CommentReplyDto
    {
        public int ParentCommentId { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
    }
}
