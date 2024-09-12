namespace Final.Mvc.Areas.AdminArea.ViewModels.Categories
{
    public class CategoryListVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GameCount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

    }
}
