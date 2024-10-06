namespace Final.Mvc.Areas.AdminArea.ViewModels.CommnetVMs
{
    public class AdminCommentDetailVM
    {
        public int Id { get; set; }
        //public string UserName { get; set; }
        public string GameTitle { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<CommentHistoryVM> CommentHistories { get; set; }
    }
    public class CommentHistoryVM
    {
        public string PreviousContent { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
