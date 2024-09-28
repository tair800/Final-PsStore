namespace Final.Mvc.ViewModels.WishlistVMs
{
    public class UserWishlistVM
    {
        public string UserId { get; set; }
        public List<WishlistGameVM> WishlistGames { get; set; } = new List<WishlistGameVM>();
        public decimal TotalPrice => WishlistGames?.Sum(wg => wg.Price) ?? 0;
    }

    public class WishlistGameVM
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string GameImgUrl { get; set; }
    }
}
