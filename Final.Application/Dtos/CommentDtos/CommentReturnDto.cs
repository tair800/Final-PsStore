namespace Final.Application.Dtos.CommentDtos
{
    public class CommentReturnDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public int GameId { get; set; }
        public string Username { get; set; } // New property for username

        public DateTime CreatedAt { get; set; }
    }
}
