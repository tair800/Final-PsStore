using System.ComponentModel.DataAnnotations;

namespace Final.Mvc.Areas.AdminArea.ViewModels.DlcVMs
{
    public class AdminDlcCreateVM
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int GameId { get; set; }

        public List<AdminGameVM> Games { get; set; } = new List<AdminGameVM>();
    }

    public class AdminGameVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
