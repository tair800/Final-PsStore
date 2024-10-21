using System.ComponentModel.DataAnnotations;

namespace Final.Mvc.Areas.AdminArea.ViewModels.Categories
{
    public class AdminCategoryUpdateVM
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string Name { get; set; }
    }
}
