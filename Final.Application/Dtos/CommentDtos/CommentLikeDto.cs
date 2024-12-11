namespace Final.Application.Dtos.CommentDtos
{
    public class CommentLikeDto
    {
        public int CommentId { get; set; }
        public bool IsLike { get; set; }
        public string UserId { get; set; }
    }
}
