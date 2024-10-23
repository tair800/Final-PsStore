using Final.Mvc.Areas.AdminArea.ViewModels.DlcVMs;

public class AdminDlcUpdateVM
{
    public int Id { get; set; }

    public string Name { get; set; }

    public double Price { get; set; }

    public int GameId { get; set; }

    public IFormFile? File { get; set; }  // For uploading a new image

    public string? ImgUrl { get; set; }  // For displaying the existing image

    public List<AdminGameVM> Games { get; set; }  // Dropdown of available games
}
