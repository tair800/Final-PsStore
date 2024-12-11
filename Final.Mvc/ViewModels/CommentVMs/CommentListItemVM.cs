using Final.Mvc.ViewModels.GameVMs;

namespace Final.Mvc.ViewModels.CommentVMs
{
    public class CommentListItemVM
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Like { get; set; }
        public string Dislike { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool CanDelete { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }

    }

    public class GameDetailWithCommentsVM
    {
        public GameDetailVM GameDetail { get; set; }
        public List<CommentListItemVM> Contents { get; set; }
        public CommentCreateVM ContentNew { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }


    }
}



