namespace Final.Mvc.ViewModels.WishlistVMs
{
    public class WishlistVM
    {
        public string UserId { get; set; }
        public List<WishlistItemVM> Items { get; set; } = new List<WishlistItemVM>();
    }

    public class WishlistItemVM
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }
    }
}
