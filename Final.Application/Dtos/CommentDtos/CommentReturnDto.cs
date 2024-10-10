namespace Final.Application.Dtos.CommentDtos
{
    public class CommentReturnDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public int GameId { get; set; }
        public string Username { get; set; } // New property for username
        public string GameTitle { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        //public List<CommentHistoryDto> CommentHistories { get; set; } // Add CommentHistories here

    }
}
