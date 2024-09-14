namespace Final.Mvc.Areas.AdminArea.ViewModels.GameVMs
{
    public class GameListVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImgUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
