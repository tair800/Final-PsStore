namespace Final.Mvc.Areas.AdminArea.ViewModels.DlcVMs
{
    public class AdminDlcCreateVM
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int GameId { get; set; }

        public List<AdminGameVM> Games { get; set; } = new List<AdminGameVM>();
        public IFormFile Image { get; set; }

    }

    public class AdminGameVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
