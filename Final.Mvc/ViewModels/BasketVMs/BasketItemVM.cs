namespace Final.Mvc.ViewModels.BasketVMs
{
    public class UserBasketVM
    {
        public string UserId { get; set; }
        public List<BasketGameVM> BasketGames { get; set; }
        public decimal TotalPrice => BasketGames?.Sum(bg => bg.Price * bg.Quantity) ?? 0;

    }
    public class BasketGameVM
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string GameImgUrl { get; set; }
        public decimal SalePrice { get; set; }
    }
}
