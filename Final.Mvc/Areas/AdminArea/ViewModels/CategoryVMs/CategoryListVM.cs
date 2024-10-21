namespace Final.Mvc.Areas.AdminArea.ViewModels.Categories
{
    public class CategoryListVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GameCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
