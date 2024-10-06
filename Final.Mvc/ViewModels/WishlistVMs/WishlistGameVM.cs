namespace Final.Mvc.ViewModels.WishlistVMs
{
    public class WishlistGamesVM
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; } // Optional if applicable
        public string GameImgUrl { get; set; }
    }
}
