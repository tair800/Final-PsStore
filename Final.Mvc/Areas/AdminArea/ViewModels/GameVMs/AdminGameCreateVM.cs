using System.ComponentModel.DataAnnotations;

namespace Final.Mvc.Areas.AdminArea.ViewModels.GameVMs
{
    public class AdminGameCreateVM
    {
        [Required]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal? SalePrice { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public Platform Platform { get; set; }

        public IFormFile ImgUrl { get; set; }

        public List<AdminCategoryVM> Categories { get; set; }
    }

    public class AdminCategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public enum Platform
    {
        PS4,
        PS5,
        PS4PS5
    }
}
