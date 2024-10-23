namespace Final.Mvc.Areas.AdminArea.ViewModels.GameVMs
{
    public class AdminGameUpdateVM
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? SalePrice { get; set; }
        public IFormFile? File { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ImgUrl { get; set; }
        public Platforms? Platform { get; set; }
        public List<int> DlcIds { get; set; } = new List<int>(); // New property to handle selected DLCs
        public List<DlcVM> AvailableDlcs { get; set; } = new List<DlcVM>(); // List of available DLCs to display in the view
        public List<CategoryVM> Categories { get; set; } = new();
    }
    public class DlcVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class CategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public enum Platforms
    {
        PS4,
        PS5,
        PS4PS5
    }
}
