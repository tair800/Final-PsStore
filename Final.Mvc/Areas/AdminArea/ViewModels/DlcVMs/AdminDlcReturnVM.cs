namespace Final.Mvc.Areas.AdminArea.ViewModels.DlcVMs
{
    public class AdminDlcReturnVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
