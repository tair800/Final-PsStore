using System.ComponentModel.DataAnnotations;

namespace Final.Mvc.Areas.AdminArea.ViewModels.GameVMs
{
    public class AdminGameCreateVM
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000.")]
        public decimal Price { get; set; }

        [Range(0.01, 10000, ErrorMessage = "Sale Price must be between 0.01 and 10000.")]
        public decimal? SalePrice { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Platform is required.")]
        public string Platform { get; set; }

        [Required(ErrorMessage = "An image is required.")]
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
