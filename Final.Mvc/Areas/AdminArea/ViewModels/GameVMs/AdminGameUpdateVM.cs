
using Final.Mvc.ViewModels.GameVMs;

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
        public int? CategoryId { get; set; }
        public string ImgUrl { get; set; }
        public Platform? Platform { get; set; }

        public List<CategoryVM> Categories { get; set; }
    }

    public class CategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
