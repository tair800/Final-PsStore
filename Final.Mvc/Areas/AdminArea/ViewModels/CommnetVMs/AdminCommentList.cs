namespace Final.Mvc.Areas.AdminArea.ViewModels.CommnetVMs
{
    public class AdminCommentListVM
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}
