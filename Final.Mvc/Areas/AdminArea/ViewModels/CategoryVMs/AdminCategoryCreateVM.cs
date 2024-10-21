using System.ComponentModel.DataAnnotations;

namespace Final.Mvc.Areas.AdminArea.ViewModels.Categories
{
    public class AdminCategoryCreateVM
    {
        [Required]
        public string Name { get; set; }
    }
}
