using Final.Mvc.ViewModels.GameVMs;

namespace Final.Mvc.ViewModels.CommentVMs
{
    public class CommentListItemVM
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; } // For displaying the username
        public DateTime CreatedDate { get; set; }
        public bool CanDelete { get; set; }  // To
        public DateTime? UpdatedDate { get; set; }

    }

    public class GameDetailWithCommentsVM
    {
        public GameDetailVM GameDetail { get; set; }  // Represents game-specific information
        public List<CommentListItemVM> Contents { get; set; }  // List of comments
        public CommentCreateVM ContentNew { get; set; }  // Model for adding new comment
    }
}



