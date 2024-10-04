using Final.Mvc.ViewModels.GameVMs;

namespace Final.Mvc.ViewModels.CommentVMs
{
    public class CommentListItemVM
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public bool CanDelete { get; set; } // Add this property
        public string Username { get; set; } // New property for username

        public bool Modified { get; set; }
        public DateTime CreateDate { get; set; }

    }
    public class GameDetailWithCommentsVM
    {
        public GameDetailVM GameDetail { get; set; }
        public List<CommentListItemVM> Contents { get; set; }
        public CommentCreateVM ContentNew { get; set; }
    }

}
