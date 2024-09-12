namespace Final.Mvc.Areas.AdminArea.ViewModels.Categories
{
    public class AdminCategoryReturn
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> GameNames { get; set; } = new List<string>();
        public int GameCount { get; set; }
    }
}
